using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericStageInjectionCommon.Shared;

namespace GenericStageInjection
{
    /// <summary>
    /// Contains the individual objects that form an individual internally injectable stage.
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// Stores the individual stage configuration set by the user.
        /// </summary>
        public Config StageConfig;

        /// <summary>
        /// Holds an array to the current spline table terminated with a null pointer.
        /// </summary>
        public Spline[] Splines;

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
            Splines = new Spline[splineFiles.Length + 1];   // Spline is a class, thus each entry will be a pointer to a Spline instance.
                                                            // The game uses a null pointer to determine the end of the spline entries to be used in the spline table.
                                                            // Therefore we create 1 more entry than the list of splines to load, so it has a null pointer.
            // Populate Splines.
            for (int x = 0; x < splineFiles.Length; x++)
                Splines[x] = Spline.MakeSpline(splineFiles[x]);
        }
    }
}
