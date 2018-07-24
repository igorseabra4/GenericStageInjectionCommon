using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GenericStageInjectionCommon.Structs;
using GenericStageInjectionCommon.Structs.Enums;
using GenericStageInjectionCommon.Structs.Extensions;

namespace GenericStageInjectionCommon
{
    public class Spline
    {
        /// <summary>
        /// Struct that defines a spline header.
        /// </summary>
        public SplineHeader Header;

        /// <summary>
        /// Individual Vertices of the Spline File.
        /// </summary>
        public List<SplineVertex> Vertices;
        
        public static Spline FromFile(string fileName)
        {
            // Set Culture
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Spline SplineFile = new Spline();

            // Properties
            bool CalculateSplineDistanceFlag = false;
            float SplineVertexDistance = 0.0F;
            int SplineVertexFlags = 0;

            // Defaults
            SplineFile.Header.SplineType = SplineType.Null;

            // Read Spline File
            SplineFile.Vertices = new List<SplineVertex>(); // List of vertices for spline.
            string[] OBJ_File = File.ReadAllLines(fileName); // Read all of the lines.
            string[] Vertex_XYZ; // Used when splitting current lines of OBJ Files.

            // Iterate over all lines.
            for (int i = 0; i < OBJ_File.Length; i++)
            {
                try
                {
                    string Value = OBJ_File[i].Substring(OBJ_File[i].IndexOf("=") + 1);

                    // Check Spline Type
                    if (OBJ_File[i].StartsWith("SPLINE_TYPE"))
                    {
                        if (Value == "Null") { SplineFile.Header.SplineType = SplineType.Null; }
                        else if (Value == "Loop") { SplineFile.Header.SplineType = SplineType.Loop; }
                        else if (Value == "Rail") { SplineFile.Header.SplineType = SplineType.Rail; }
                        else if (Value == "Ball") { SplineFile.Header.SplineType = SplineType.Ball; }
                    }
                    else if (OBJ_File[i].StartsWith("SPLINE_VERTEX_FLAGS"))
                    {
                        SplineVertexFlags = Convert.ToInt32(Value, 16);
                    }
                    else if (OBJ_File[i].StartsWith("DISTANCE_TO_NEXT_POINT"))
                    {
                        if (Value == "Auto") { CalculateSplineDistanceFlag = true; }
                        else { SplineVertexDistance = Convert.ToSingle(Value); }
                    }
                    else if (OBJ_File[i].StartsWith("v"))
                    {
                        Vertex_XYZ = Regex.Replace(OBJ_File[i], @"\s+", " ").Split();

                        // Define a new Vertex.
                        SplineVertex TempVertex = new SplineVertex(Convert.ToSingle(Vertex_XYZ[1]), Convert.ToSingle(Vertex_XYZ[2]), Convert.ToSingle(Vertex_XYZ[3]));
                        
                        // Set Flags
                        TempVertex.UnknownRotation = SplineVertexFlags;

                        // Set Distance between Verts.
                        if (!CalculateSplineDistanceFlag) { TempVertex.DistanceToNextVertex = SplineVertexDistance; }
                        else { TempVertex.DistanceToNextVertex = 0.0F; }

                        SplineFile.Vertices.Add(TempVertex);
                    }
                }
                catch { }
            }
            
            // Set properties
            SplineFile.Header.Enabler = 1;
            SplineFile.Header.NumberOfVertices = (ushort)SplineFile.Vertices.Count;

            // Calculate Distance to Next Vertex if Auto
            if (CalculateSplineDistanceFlag)
            {
                for (int x = 0; x < SplineFile.Vertices.Count - 1; x++)
                {
                    SplineVertex TempVertex = SplineFile.Vertices[x];
                    TempVertex.DistanceToNextVertex = SplineVertexExtensions.CalculateDistanceToNext(SplineFile.Vertices[x], SplineFile.Vertices[x + 1]);
                    SplineFile.Vertices[x] = TempVertex;
                }
            }

            // Calculate Spline Length
            SplineFile.Header.TotalSplineLength = 0F;
            for (int x = 0; x < SplineFile.Vertices.Count; x++) { SplineFile.Header.TotalSplineLength += SplineFile.Vertices[x].DistanceToNextVertex; }

            return SplineFile;
        }
    }
}
