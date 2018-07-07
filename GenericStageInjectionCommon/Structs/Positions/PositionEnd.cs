namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Represents a coordinate whereby an individual team performs their ending poses after finishing a stage.
    /// </summary>
    public struct PositionEnd
    {
        /// <summary>
        /// Position where the animation is performed.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The vertical angle of their poses expressed in BAMS.
        /// </summary>
        public int Pitch;

        public int Null;
    }
}