namespace GenericStageInjectionCommon.Structs
{
    /// <summary>
    /// Represents a standard vertex in three dimentional space.
    /// </summary>
    public struct Triangle
    {
        public Vector Vertex1;
        public Vector Vertex2;
        public Vector Vertex3;

        public Triangle(Vector Vertex1, Vector Vertex2, Vector Vertex3)
        {
            this.Vertex1 = Vertex1;
            this.Vertex2 = Vertex2;
            this.Vertex3 = Vertex3;
        }
    }
}