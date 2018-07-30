using System;
using System.Collections.Generic;
using System.Text;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions.Substructures;
using Newtonsoft.Json;

namespace GenericStageInjectionCommon.Shared
{
    /// <summary>
    /// Represents an individual level entru inside of the readable/writable configuration file
    /// of starting and ending positions.
    /// </summary>
    [JsonObject]
    public class ConfigEntry
    {
        /// <summary>
        /// The individual stage ID for the current stage. You can find the list of Stage IDs on SCHG.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public StageID StageId;

        /// <summary>
        /// Contains a set of 5 entries for each of the four teams and one unused team.
        /// For multiplayer stages, this is treated as a set of 2 entries for P1 and P2.
        /// Note: Use <see cref="Teams"/> as an indexer for reading/writing 1P stuff.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionStart> StartPositions;

        /// <summary>
        /// Contains a set of 5 entries for each of the four teams and one unused team.
        /// For multiplayer stages, this is treated as set of 2 entries for P1 and P2.
        /// Note: Use <see cref="Teams"/> both as the order of reading and writing of this list.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionEnd> EndPositions;

        /// <summary>
        /// Contains a set of 4 entries for each of the individual teams.
        /// Note: Ignored for 1P Stages.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionEnd> BragPositions;
    }
}
