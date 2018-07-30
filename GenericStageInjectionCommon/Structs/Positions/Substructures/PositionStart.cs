using GenericStageInjectionCommon.Structs.Enums;

namespace GenericStageInjectionCommon.Structs.Positions.Substructures
{
    /// <summary>
    /// Describes the position and how the player starts off the stage.
    /// </summary>
    public struct PositionStart
    {
        /// <summary>
        /// The starting position of the player.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The vertical direction of the position expressed in BAMS.
        /// </summary>
        public int Pitch;

        public int UnknownUnused;

        /// <summary>
        /// Describes how the player starts off the stage.
        /// </summary>
        public StartPositionMode Mode;

        /// <summary>
        /// Time spent running in Running mode or without controller control, in frames.
        /// </summary>
        public int HoldTime;
    }
}
