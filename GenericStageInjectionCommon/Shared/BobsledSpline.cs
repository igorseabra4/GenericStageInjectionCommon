using System;
using System.Linq;
using System.Runtime.InteropServices;
using GenericStageInjectionCommon.Shared.Misc;
using GenericStageInjectionCommon.Shared.Parsers;
using GenericStageInjectionCommon.Structs;
using GenericStageInjectionCommon.Structs.Enums;
using GenericStageInjectionCommon.Structs.Splines;

namespace GenericStageInjectionCommon.Shared
{
    /// <summary>
    /// Struct that defines a spline header.
    /// </summary>
    public unsafe struct BobsledSpline
    {
        /// <summary>
        /// Always 0
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
        public Vector* VertexList;

        /// <summary>
        /// Cast Spline_Type
        /// </summary>
        public int Null;

        /// <summary>
        /// Instantiates a spline from an individual OBJ file which contains a set of vertices representing the individual points of the spline.
        /// </summary>
        /// <param name="path">An OBJ file containing the points of the spline as vertices.</param>
        public static BobsledSpline* MakeSpline(string path)
        {
            // Store newly created Spline
            BobsledSpline spline = new BobsledSpline();
            
            // Parse individual spline file.
            ObjParser parser = new ObjParser(path);

            // Spline Information
            spline.Enabler = 0;
            spline.Null = 0;
            spline.NumberOfVertices = (ushort)parser.Vertices.Count;

            // Get vertices & calculate total length
            Vector[] vertices = parser.Vertices.ToArray();

            spline.TotalSplineLength = 0;
            for (int i = 0; i < vertices.Length - 1; i++)
                spline.TotalSplineLength += SplineVertexExtensions.Distance(vertices[i], vertices[i + 1]);
            
            // Write vertices to unmanaged memory
            int structSize = Marshal.SizeOf(vertices[0]) * vertices.Length;
            IntPtr splineVerticesPointer = Marshal.AllocHGlobal(structSize);
            MarshalUtilities.StructureArrayToPointer(vertices, splineVerticesPointer);
            spline.VertexList = (Vector*) splineVerticesPointer;

            // Write spline to unmanaged memory
            int splineSize = Marshal.SizeOf(spline);
            IntPtr splinePointer = Marshal.AllocHGlobal(splineSize);
            Marshal.StructureToPtr(spline, splinePointer, true);

            return (BobsledSpline*)splinePointer;
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
    }
}
