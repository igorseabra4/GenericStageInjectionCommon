using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions.Substructures;

namespace GenericStageInjectionCommon.Structs.Positions
{
    /// <summary>
    /// Describes a Multiplayer Bragging Position structure for an individual action stage.
    /// Bragging just describes the short comment a team makes to another at the start of a 2P stage.
    /// </summary>
    public struct MultiplayerBrag
    {
        public StageID StageID;
        public PositionEnd Sonic;
        public PositionEnd Dark;
        public PositionEnd Rose;
        public PositionEnd Chaotix;
    }
}
