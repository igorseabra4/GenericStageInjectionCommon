using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GenericStageInjectionCommon.Shared.Parsers;
using GenericStageInjectionCommon.Structs.Enums;

namespace GenericStageInjectionCommon.Structs.Splines
{
    /// <summary>
    /// Struct that defines a spline header.
    /// </summary>
    public unsafe struct Spline
    {
        /// <summary>
        /// Always 1
        /// </summary>
        public ushort Enabler;
        
        /// <summary>
        /// Amount of vertices in spline
        /// </summary>
        public ushort NumberOfVertices;
        
        /// <summary>
        /// Purpose Unknown, Set 1000F
        /// </summary>
        public float TotalSplineLength;

        /// <summary>
        /// Points to the vertex list for the current individual spline.
        /// </summary>
        public SplineVertex* VertexList;

        /// <summary>
        /// Cast Spline_Type
        /// </summary>
        public SplineType SplineType;

        /// <summary>
        /// Instantiates a spline from an individual OBJ file which contains a set of vertices representing the individual points of the spline.
        /// </summary>
        /// <param name="path">An OBJ file containing the points of the spline as vertices.</param>
        public static Spline* MakeSpline(string path)
        {
            // Store newly created Spline
            Spline spline = new Spline();
            
            // Parse individual spline file.
            ObjParser parser = new ObjParser(path);

            // Spline Information
            spline.Enabler = 1;
            spline.SplineType = parser.GetSplineType();
            spline.NumberOfVertices = (ushort)parser.Vertices.Count;

            // Get vertices & calculate total length
            var splineVertices = MakeSplineVertices(ref parser);
            spline.TotalSplineLength = splineVertices.Sum(x => x.DistanceToNextVertex);

            // Allocate memory for spline vertices and write them.
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(Marshal.SizeOf(splineVertices));
            spline.VertexList = (SplineVertex*)unmanagedPointer;
            Marshal.StructureToPtr(splineVertices, unmanagedPointer, false);

            // Allocate self and return pointer.
            IntPtr splinePointer = Marshal.AllocHGlobal(Marshal.SizeOf(spline));
            Marshal.StructureToPtr(spline, splinePointer, false);
            return (Spline*)splinePointer;
        }

        /// <summary>
        /// Dispose of the current spline from memory to leave no residue
        /// in unmanaged memory.
        /// </summary>
        public static void DestroySpline(Spline* spline)
        {
            Marshal.FreeHGlobal((IntPtr)(*spline).VertexList);
            Marshal.FreeHGlobal((IntPtr)spline);
        }

        /// <summary>
        /// Creates an array of Sonic Heroes native spline vertices from a supplied parsed OBJ file.
        /// </summary>
        /// <param name="parser">An instance of the OBJ parser containing spline points as its list of vertices.</param>
        /// <returns>List of processed spline vertices.</returns>
        private static SplineVertex[] MakeSplineVertices(ref ObjParser parser)
        {
            // Generate list of vertices for spline.
            var vertices = new SplineVertex[parser.Vertices.Count];
            for (int x = 0; x < parser.Vertices.Count; x++)
                vertices[x] = new SplineVertex(parser.Vertices[x]);

            // Calculate total spline length.
            int loopMaximum = vertices.Length - 1;

            // Set distance to each next vertex.
            for (int x = 0; x < loopMaximum; x++)
                vertices[x].DistanceToNextVertex = vertices[x].CalculateDistanceToNext(vertices[x + 1]);

            return vertices;
        }
    }
}
