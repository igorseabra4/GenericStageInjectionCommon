using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Structs.Enums;

namespace GenericStageInjectionCommon.Shared.Parsers
{
    /// <summary>
    /// Provides various extension methods used in tandem with our own individual spline files.
    /// </summary>
    public static class ObjParserSplineExtensions
    {
        /// <summary>
        /// Gets the spline type for the current OBJ file (if it is a spline).
        /// </summary>
        /// <param name="objParser">The OBJ file to try and determine a spline type from.</param>
        /// <returns>The spline type for this OBJ file.</returns>
        public static SplineType GetSplineType(this ObjParser objParser)
        {
            // Iterate over all lines.
            foreach (var line in objParser.ObjTextFile)
            {
                // Check if we have found our desired line.
                if (line.StartsWith("SPLINE_TYPE"))
                {
                    // Get value assigned to this entry.
                    string value = line.Substring(line.IndexOf("=", StringComparison.Ordinal) + 1);

                    if (value == "Loop")        { return SplineType.Loop; }
                    if (value == "Rail")        { return SplineType.Rail; }
                    if (value == "Ball")        { return SplineType.Ball; }
                }
            }

            // Default
            return SplineType.Loop;
        }


    }
}
