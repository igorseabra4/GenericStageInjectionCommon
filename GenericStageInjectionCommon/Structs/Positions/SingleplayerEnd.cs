using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions.Substructures;

namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Describes a Singleplayer End Position structure for an individual action stage.
    /// </summary>
    public struct SingleplayerEnd
    {
        /// <summary>
        /// The stage the following start/end position is for.
        /// </summary>
        public StageID StageID;

        /// <summary>
        /// Team Sonic's ending position.
        /// </summary>
        public PositionEnd SonicEnd;

        /// <summary>
        /// Team Dark's ending position.
        /// </summary>
        public PositionEnd DarkEnd;

        /// <summary>
        /// Team Rose ending position.
        /// </summary>
        public PositionEnd RoseEnd;

        /// <summary>
        /// Team Chaotix' ending position.
        /// </summary>
        public PositionEnd ChaotixEnd;

        /// <summary>
        /// Unused Team
        /// </summary>
        public PositionEnd ForeditEnd;
    }
}
