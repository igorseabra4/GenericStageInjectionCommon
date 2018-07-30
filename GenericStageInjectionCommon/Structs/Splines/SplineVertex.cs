namespace GenericStageInjectionCommon.Structs.Splines
{
    /// <summary>
    /// Represents an individual vertex of a SADX/SA2/Heroes spline.
    /// </summary>
    public struct SplineVertex
    {
        /// <summary>
        /// Believed to simply be a polar angle to the next vertex.
        /// </summary>
        public int UnknownRotation;

        /// <summary>
        /// Calculated through taking away the XYZ postion of the next vector with the current vector and then
        /// performing an addition of the squared XYZ components and taking the square root.
        /// e.g. sqrt((X1 - X2)^2 + (Y1 - Y2)^2 + (Z1 - Z2)^2)
        /// </summary>
        public float DistanceToNextVertex;

        /// <summary>
        /// Represents the position of the spline's vertex in 3D space.
        /// </summary>
        public Vector Position;

        public SplineVertex(float X, float Y, float Z) : this(new Vector(X, Y, Z))
        {
        }

        /// <summary>
        /// Creates an instance of a vertex from a mutually predefined position.
        /// </summary>
        /// <param name="position">The initial position of the vertex to write to.</param>
        public SplineVertex(Vector position)
        {
            this.Position = position;
            DistanceToNextVertex = 0;
            UnknownRotation = 0;
        }
    }
}