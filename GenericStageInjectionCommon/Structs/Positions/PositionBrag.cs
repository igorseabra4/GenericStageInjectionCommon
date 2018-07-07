namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Represents a coordinate whereby an individual team performs their entrance bragging poses in multiplayer mode.
    /// </summary>
    public struct PositionBrag
    {
        /// <summary>
        /// Position where the bragging is performed.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The vertical angle of their poses expressed in BAMS.
        /// </summary>
        public int Pitch;
    }
}