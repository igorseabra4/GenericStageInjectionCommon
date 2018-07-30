using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions.Substructures;

namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Describes a Singleplayer Start Position structure for an individual action stage.
    /// </summary>
    public struct SingleplayerStart
    {
        /// <summary>
        /// The stage the following start position is for,
        /// </summary>
        public StageID StageID;

        /// <summary>
        /// Team Sonic's starting position.
        /// </summary>
        public PositionStart SonicStart;

        /// <summary>
        /// Team Dark's starting position.
        /// </summary>
        public PositionStart DarkStart;

        /// <summary>
        /// Team Rose starting position.
        /// </summary>
        public PositionStart RoseStart;

        /// <summary>
        /// Team Chaotix' starting position.
        /// </summary>
        public PositionStart ChaotixStart;

        /// <summary>
        /// Unused Team
        /// </summary>
        public PositionStart ForeditStart;
    }
}
