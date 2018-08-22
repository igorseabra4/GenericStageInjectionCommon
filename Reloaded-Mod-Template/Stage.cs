using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GenericStageInjectionCommon.Shared;

namespace GenericStageInjection
{
    /// <summary>
    /// Contains the individual objects that form an individual internally injectable stage.
    /// </summary>
    public unsafe class Stage : IDisposable
    {
        /// <summary>
        /// Stores the individual stage configuration set by the user.
        /// </summary>
        public Config StageConfig;

        /// <summary>
        /// Holds an array to the current spline table terminated with a null pointer.
        /// </summary>
        public Spline** Splines;

        /// <summary>
        /// Contains the number of splines that are
        /// </summary>
        public int SplineCount;

        /// <summary>
        /// Defines the subfolder used for storing spline content.
        /// </summary>
        public string SplineFolderPath => _stageDirectory + "\\Splines";

        /// <summary>
        /// Defines the subfolder used for storing spline content.
        /// </summary>
        public string FilesFolderPath => _stageDirectory + "\\Files";

        /// <summary>
        /// Defines the file name used for storing the current stage's configuration properties.
        /// </summary>
        public string ConfigFilePath => _stageDirectory + "\\Stage.json";

        /// <summary>
        /// Contains the directory containing the individual stage's files.
        /// </summary>
        private readonly string _stageDirectory;

        /// <summary>
        /// Creates an instance of a stage from a provided directory containing stage files.
        /// </summary>
        /// <param name="stageDirectory">Contains the full path leading to individual stage content.</param>
        public Stage(string stageDirectory)
        {
            // Set the stage directory.
            _stageDirectory = stageDirectory;

            // Set the config for the stage.
            StageConfig = Config.ParseConfig(ConfigFilePath);

            // Get all splines.
            string[] splineFiles = Directory.GetFiles(SplineFolderPath, "*.obj");

            // Allocate unmanaged memory for splines
            int pointerSize = sizeof(Spline*);
            int splineArraySize = pointerSize * (splineFiles.Length + 1);
            Splines = (Spline**)Marshal.AllocHGlobal(splineArraySize); 
            
            // We allocate one more than required because the last one should be a null pointer.
            // The game uses a null pointer to determine end of array.

            // Populate Splines.
            for (int x = 0; x < splineFiles.Length; x++)
                Splines[x] = Spline.MakeSpline(splineFiles[x]);

            Splines[splineFiles.Length] = (Spline*)0;
            SplineCount = splineFiles.Length;
        }

        /// <summary>
        /// Disposes the unmanaged resources from memory.
        /// </summary>
        public void Dispose()
        {
            for (int x = 0; x < SplineCount; x++)
            {
                Spline.DestroySpline(Splines[x]);
            }
            Marshal.FreeHGlobal((IntPtr)Splines);
        }
    }
}
