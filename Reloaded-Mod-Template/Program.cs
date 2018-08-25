using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GenericStageInjectionCommon.Shared;
using GenericStageInjectionCommon.Shared.Ingame;
using Reloaded;
using Reloaded.IO;
using Reloaded.Process;
using Reloaded.Process.Functions.X86Hooking;
using Reloaded.Process.Native;
using static GenericStageInjection.Hooks;

namespace GenericStageInjection
{
    public static unsafe class Program
    {
        #region Disclaimer
        /*
         *  Reloaded Mod Loader DLL Modification Template
         *  Sewer56, 2018 ©
         *
         *  -------------------------------------------------------------------------------
         *
         *  Here starts your own mod loader DLL code.
         *
         *  The Init function below is ran at the initialization stage of the game.
         *
         *  The game at this point suspended and frozen in memory. There is no execution
         *  of game code currently ongoing.
         *
         *  This is where you do your hot-patches such as graphics stuff, patching the
         *  window style of the game to borderless, setting up your initial variables, etc.
         *
         *  -------------------------------------------------------------------------------
         *
         *  Important Note:
         *
         *  This function is executed once during startup and SHOULD return as the
         *  mod loader awaits successful completion of the main function.
         *
         *  If you want your mod/code to sit running in the background, please initialize
         *  another thread and run your code in the background on that thread, otherwise
         *  please remember to return from the function.
         *
         *  There is also some extra code, including DLL stubs for Reloaded, classes
         *  to interact with the Mod Loader Server as well as other various loader related
         *  utilities available.
         *
         *  -------------------------------------------------------------------------------
         *  Extra Tip:
         *
         *  For Reloaded mod development, there are also additional libraries and packages
         *  available on NuGet which provide you with extra functionality.
         *
         *  Examples include:
         *  [Input] Reading controller information using Reloaded's input stack.
         *  [IO] Accessing the individual Reloaded config files.
         *  [Overlays] Easy to use D3D and external overlay code.
         *
         *  Simply search libReloaded on NuGet to find those extras and refer to
         *  Reloaded-Mod-Samples subdirectory on Github for examples of using them (and
         *  sample mods showing how Reloaded can be used).
         *
         *  -------------------------------------------------------------------------------
         *
         *  [Template] Brief Walkthrough:
         *
         *  > ReloadedCode/Initializer.cs
         *      Stores Reloaded Mod Loader DLL Template/Initialization Code.
         *      You are not required/should not (need) to modify any of the code.
         *
         *  > ReloadedCode/Client.cs
         *      Contains various pieces of code to interact with the mod loader server.
         *
         *      For convenience it's recommended you import Client static(ally) into your
         *      classes by doing it as such `Reloaded_Mod_Template.Reloaded_Code.Client`.
         *
         *      This will avoid you typing the full class name and let you simply type
         *      e.g. Print("SomeTextToConsole").
         *
         *  -------------------------------------------------------------------------------
         *
         *  If you like Reloaded, please consider giving a helping hand. This has been
         *  my sole project taking up most of my free time for months. Being the programmer,
         *  artist, tester, quality assurance, alongside various other roles is pretty hard
         *  and time consuming, not to mention that I am doing all of this for free.
         *
         *  Well, alas, see you when Reloaded releases.
         *
         *  Please keep this notice here for future contributors or interested parties.
         *  If it bothers you, consider wrapping it in a #region.
        */
        #endregion Disclaimer

        #region Reloaded Template Default Variables
        /*
            Default Variables:
            These variables are automatically assigned by the mod template, you do not
            need to assign those manually.
        */

        /// <summary>
        /// Holds the game process for us to manipulate.
        /// Allows you to read/write memory, perform pattern scans, etc.
        /// See libReloaded/GameProcess (folder)
        /// </summary>
        public static ReloadedProcess GameProcess;

        /// <summary>
        /// Stores the absolute executable location of the currently executing game or process.
        /// </summary>
        public static string ExecutingGameLocation;

        /// <summary>
        /// Specifies the full directory location that the current mod 
        /// is contained in.
        /// </summary>
        public static string ModDirectory;
        #endregion

        /// <summary>
        /// Contains the list of individual stages
        /// that are about to be loaded/injected.
        /// </summary>
        private static List<Stage> _stages = new List<Stage>();
        
        /// <summary>
        /// Hooks the game's InitPath function in order to 
        /// replace the splines about to be loaded.
        /// </summary>
        private static FunctionHook<InitPath> _initPathHook;

        /// <summary>
        /// Hooks the Windows API NtCreateFile hook in order to provide file redirection to each of the individual mod
        /// folders.
        /// </summary>
        private static FunctionHook<NtCreateFile> _ntCreateFileHook;

        /// <summary>
        /// Maps file paths to be accessed by the game to another file path which
        /// belongs to an individual stage.
        /// </summary>
        private static Dictionary<string, string> _remapperDictionary;

        /// <summary>
        /// Used to store a singular unique filesystem watcher for each individual stage folder, Files directory.
        /// Fires events which cause a rebuild of the file redirection dictionary on file additions, removals and overrides.
        /// </summary>
        private static Dictionary<Stage, FileSystemWatcher> _fileSystemWatcherDictionary = new Dictionary<Stage, FileSystemWatcher>();

        /// <summary>
        /// Read all of the individual game configurations and their corresponding spline lists.
        /// </summary>
        public static unsafe void Init()
        {
            # if DEBUG
            Debugger.Launch();
            #endif

            // Just in case.
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // TODO: General library for hacking Sonic Heroes from which to source 0x00439020, 0x8D6710 and other addresses from.
            // TODO: Test this program, I haven't started it once, yet.

            // Populate all Stages.
            string[] stageDirectories = Directory.GetDirectories(ModDirectory + "\\Stages\\");

            // Load all stages.
            foreach (var stageDirectory in stageDirectories)
                _stages.Add(new Stage(stageDirectory));

            // Get the pointers for the individual stage then apply a new spawn configuration for them.
            foreach (var stage in _stages)    
                StageInfo.ApplyConfig(StageInfo.GetStageInfo(stage.StageConfig.StageId), stage.StageConfig);
            
            // Setup file redirections 
            BuildFileRedirectionDictionary(_stages);

            // Setup spline load hook.
            _initPathHook = FunctionHook<InitPath>.Create(0x00439020, InitPathImpl).Activate();

            // Get the address of the WinAPI NtCreateFile Windows function inside ntdll and hook it.
            IntPtr ntdllHandle = Native.LoadLibraryW("ntdll");
            IntPtr ntCreateFilePointer = Native.GetProcAddress(ntdllHandle, "NtCreateFile");
            _ntCreateFileHook = FunctionHook<NtCreateFile>.Create((long)ntCreateFilePointer, FileRedirectionHook).Activate();
        }

        /// <summary>
        /// Contains the implementation of the NtCreateFile hook.
        /// Conditionally redirects oncoming files through changing ObjectName inside the objectAttributes instance.
        /// </summary>
        private static int FileRedirectionHook(out IntPtr handle, FileAccess access, ref OBJECT_ATTRIBUTES objectAttributes, ref IO_STATUS_BLOCK ioStatus, ref long allocSize, uint fileAttributes, FileShare share, uint createDisposition, uint createOptions, IntPtr eaBuffer, uint eaLength)
        {
            // Our FIle Path.
            string filePath = GetNtCreateFilePath(ref objectAttributes);

            if (_remapperDictionary.TryGetValue(filePath, out string newFilePath))
            {
                #region DEBUG PRINT
                #if DEBUG
                Bindings.PrintInfo($"Stage Injection File Redirection: {filePath} => {newFilePath}");
                #endif
                #endregion

                // Set new filename.
                objectAttributes.ObjectName = new UNICODE_STRING("\\??\\" + newFilePath);
                objectAttributes.RootDirectory = IntPtr.Zero;
            }

            // Call original.
            return _ntCreateFileHook.OriginalFunction(out handle, access, ref objectAttributes, ref ioStatus, ref allocSize, fileAttributes, share, createDisposition, createOptions, eaBuffer, eaLength);
        }

        /// <summary>
        /// Retrieves the full file path from an OBJECT_ATTRIBUTES structure passed into the
        /// NtDll function NtCreateFile.
        /// </summary>
        /// <returns>The full file path to the file to be loaded.</returns>
        private static string GetNtCreateFilePath(ref OBJECT_ATTRIBUTES objectAttributes)
        {
            // Retrieves the file name that we are attempting to access.
            string oldFileName = objectAttributes.ObjectName.ToString();

            // Sometimes life can be a bit ugly :/
            if (oldFileName.StartsWith("\\??\\", StringComparison.InvariantCultureIgnoreCase))
            { oldFileName = oldFileName.Replace("\\??\\", ""); }

            return Path.GetFullPath(oldFileName);
        }

        /// <summary>
        /// Builds a dictionary mapping the files of each individual stage
        /// to a file in Sonic Heroes' dvdroot directory.
        /// </summary>
        /// <param name="stages">List of stages to create a mapping for.</param>
        private static void BuildFileRedirectionDictionary(List<Stage> stages)
        {
            // Local stage to stage mapping.
            Dictionary<string, string> remapperDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // We don't need to pull Reloaded-IO for this one as a dependency since we're only working with Sonic Heroes
            // TSonic_win is guaranteed to be in the same folder as dvdroot.
            string dvdrootPath = Path.GetDirectoryName(ExecutingGameLocation) + "\\dvdroot";

            // Setup remapping for each stage and 
            foreach (var stage in stages)
            {
                // Check for file redirect path existence.
                if (Directory.Exists(stage.FilesFolderPath))
                {
                    // Retrieve a listing of all files relative to this directory.
                    List<string> stageFiles = RelativePaths.GetRelativeFilePaths(stage.FilesFolderPath);

                    // Setup redirects.
                    foreach (var stageFile in stageFiles)
                    {
                        // Get old path to relocate and new replacement path.
                        string gameFilePath = dvdrootPath + stageFile;
                        string newFilePath = stage.FilesFolderPath + stageFile;

                        // Normalize the path format (ensure consistent format).
                        gameFilePath = Path.GetFullPath(gameFilePath);
                        newFilePath = Path.GetFullPath(newFilePath);

                        // Appends to the file path replacement dictionary.
                        remapperDictionary[gameFilePath] = newFilePath;
                    }

                    // Setup event based, realtime file addition/removal as new files are removed or added if not setup.
                    if (!_fileSystemWatcherDictionary.ContainsKey(stage)) // Ensure this is only done once.
                    {
                        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(stage.FilesFolderPath);
                        fileSystemWatcher.EnableRaisingEvents = true;
                        fileSystemWatcher.IncludeSubdirectories = true;
                        fileSystemWatcher.Created += (sender, args) => { BuildFileRedirectionDictionary(_stages); };
                        fileSystemWatcher.Deleted += (sender, args) => { BuildFileRedirectionDictionary(_stages); };
                        fileSystemWatcher.Renamed += (sender, args) => { BuildFileRedirectionDictionary(_stages); };
                        _fileSystemWatcherDictionary.Add(stage, fileSystemWatcher);
                    }
                }
            }

            _remapperDictionary = remapperDictionary;
        }
        
        /// <summary>
        /// Hook for Sonic Heroes' InitPath that checks the current stage ID and loads an altnernate spline set if
        /// a replaced stage is about to be loaded.
        /// </summary>
        /// <param name="splinePointerArray">
        ///     A pointer to a null pointer delimited list of pointers to the Spline structures.
        ///     C/C++: `Spline**`
        ///     
        ///     C#: ref Spline = Spline*
        ///     Note: Spline is a class, thus the actual instance stored in the array is a pointer, thus the parameter is Spline**.
        /// </param>
        /// <returns>A value of 1 or 0 for success/failure.</returns>
        private static bool InitPathImpl(Spline** splinePointerArray)
        {
            // Get current level ID.
            int currentStage = *(int*)0x8D6710;

            // Try finding a stage to inject containing the current stage's splines.
            foreach (Stage stage in _stages)
            {
                // Skip non-current stages.
                if ((int) stage.StageConfig.StageId != currentStage)
                    continue;

                // We've found our stage for whici to replace splines, call original function with our own spline set.
                return _initPathHook.OriginalFunction(stage.Splines);
            }

            // Call the original function; no injected stage requiring override found.
            return _initPathHook.OriginalFunction(splinePointerArray);
        }
    }
}
