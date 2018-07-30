using System;

namespace GenericStageInjectionCommon.Structs.Splines
{
    /// <summary>
    /// Provides extension methods for the SplineVertex struct.
    /// </summary>
    public static class SplineVertexExtensions
    {
        /// <summary>
        /// Calculates the <see cref="SplineVertex.DistanceToNextVertex"> for our individual vertex.
        /// </summary>
        public static float CalculateDistanceToNext(this SplineVertex vertex1, SplineVertex vertex2)
        {
            Vector delta = vertex1.Position - vertex2.Position;

            return (float)Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2) + Math.Pow(delta.Z, 2));
        }
    }
}