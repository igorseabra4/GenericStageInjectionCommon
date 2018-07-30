using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions.Substructures;

namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Describes a Multiplayer Start Position structure for an individual action stage.
    /// </summary>
    public struct MultiplayerStart
    {
        /// <summary>
        /// The stage the following start position is for.
        /// </summary>
        public StageID StageID;

        /// <summary>
        /// Player 1 starting position.
        /// </summary>
        public PositionStart Player1Start;

        /// <summary>
        /// Player 2 starting position.
        /// </summary>
        public PositionStart Player2Start;
    }
}
