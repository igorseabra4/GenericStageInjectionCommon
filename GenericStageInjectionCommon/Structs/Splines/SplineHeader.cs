using GenericStageInjectionCommon.Structs.Enums;

namespace GenericStageInjectionCommon.Structs
{
    /// <summary>
    /// Struct that defines a spline header.
    /// </summary>
    public struct SplineHeader
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

        public int PointerVertexList;

        /// <summary>
        /// Cast Spline_Type
        /// </summary>
        public SplineType SplineType;
    }
}
