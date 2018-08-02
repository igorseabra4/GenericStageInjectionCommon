using System;
using System.Linq;
using System.Runtime.InteropServices;
using GenericStageInjectionCommon.Shared.Misc;
using GenericStageInjectionCommon.Shared.Parsers;
using GenericStageInjectionCommon.Structs.Enums;
using GenericStageInjectionCommon.Structs.Splines;

namespace GenericStageInjectionCommon.Shared
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
            SplineVertex[] vertices = MakeSplineVertices(ref parser);
            spline.TotalSplineLength = vertices.Sum(x => x.DistanceToNextVertex);
            
            // Write vertices to unmanaged memory
            int structSize = Marshal.SizeOf(vertices[0]) * vertices.Length;
            IntPtr splineVerticesPointer = Marshal.AllocHGlobal(structSize);
            MarshalUtilities.StructureArrayToPointer(vertices, splineVerticesPointer);
            spline.VertexList = (SplineVertex*) splineVerticesPointer;

            // Write spline to unmanaged memory
            int splineSize = Marshal.SizeOf(spline);
            IntPtr splinePointer = Marshal.AllocHGlobal(splineSize);
            Marshal.StructureToPtr(spline, splinePointer, true);

            return (Spline*)splinePointer;
        }

        /// <summary>
        /// Frees a spline from unmanaged memory.
        /// </summary>
        /// <param name="spline">The spline to free from unmanaged memory.</param>
        public static void DestroySpline(Spline* spline)
        {
            Marshal.FreeHGlobal((IntPtr)spline[0].VertexList);
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
