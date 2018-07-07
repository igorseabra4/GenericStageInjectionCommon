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

        // I think doing it in this order would get us the shortest code count:

        // declare list of headers
        // foreach (spline obj file)
        //     parse vertices, calculate distance to next, etc.
        //     write vertices to memory
        //     generate header, get vertex count, count total length, write pointer to vertices and set spline type
        //     append header to list
        // Write out list of headers to memory.
        // Patch push address to list of headers.


        // Hey there!
        // I've an idea.
        // We can ditch this class and do with a slightly redefined SplineHeader and unsafe code.
        // We can redefine "public int PointerVertexList" as "public SplineVertex[] *PointerVertexList;"
        // This way we can even generate the vertex list in the constructor of the spline header, in fact, probably do an entire header in one function (which we break into smaller subfunctions).

        // won't that cause issues when writing the header to memory?
        // Never tried writing a pointer but I assume it'd just write the address of it to memory.
        // it would write the address in the mod, not the target proccess
        // We would generate the Vertex list, write it to memory with Reloaded's MemoryBuffer (more optimized, simpler version of manually allocating and writing memory)
        // and use the return address (where it was written to), assign it to *PointerVertexList;
        // Basically it would wield the same result, except we can access the vertex list without extra lines of code via an unsafe C/C++ style pointer.

        // Well yeah I guess it can be done like that
        // That said, what should we do about the settings inside our OBJ files? Keep them there?
        // It's specialized for our use so I guess it can't hurt.
        // Yeah, no need to change that
        // The alternative would be a separate file for the spline settings | Sounds unnecessary really, extra code baggage.
        // Yep, and the function can read an obj without the settings

        // On that note, lemme fetch something...

        public static Spline FromFile(string fileName)
        {
            // -------
            // I don't even get the hints for namespaces lol, have to remember stuff like System.IO manually.

            // Can you save SplineHeader? I can't edit it for some reason.
            // It's bugged till you type there I guess.
            // Or something /shrug
            // I just did
            // maybe you should work on it from your end
            // idk :3
            // I need a nap myself anyway :3
            // Yeah that'd be neat :3

            // I'm gonna close this soon so
            // I should put it on github and add you as contributor

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
                        string[] a = Regex.Replace(OBJ_File[i], @"\s+", " ").Split();

                        // Define a new Vertex.
                        SplineVertex TempVertex = new SplineVertex(Convert.ToSingle(a[1]), Convert.ToSingle(a[2]), Convert.ToSingle(a[3]));
                        
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
