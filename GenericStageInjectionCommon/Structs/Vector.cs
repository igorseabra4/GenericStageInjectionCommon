namespace GenericStageInjectionCommon.Structs
{
    /// <summary>
    /// Represents a standard vertex in three dimentional space.
    /// </summary>
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public Vector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector
            {
                X = vector1.X + vector2.X,
                Y = vector1.Y + vector2.Y,
                Z = vector1.Z + vector2.Z
            };
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector
            {
                X = vector1.X - vector2.X,
                Y = vector1.Y - vector2.Y,
                Z = vector1.Z - vector2.Z
            };
        }
    }
}