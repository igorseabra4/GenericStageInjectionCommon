using System;
using System.Collections.Generic;
using System.IO;
using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Shared.Ingame;
using GenericStageInjectionCommon.Structs.Positions;
using GenericStageInjectionCommon.Structs.Positions.Substructures;
using Newtonsoft.Json;
using static GenericStageInjectionCommon.Shared.Enums.StageTags;

namespace GenericStageInjectionCommon.Shared
{
    /// <summary>
    /// Represents an individual readable/writable configuration file of starting and ending positions 
    /// used by the Generic Stage Injection Mod DLLs and level editors such as Heroes Power Plant.
    /// </summary>
    [JsonObject]
    public unsafe class Config
    {
        /// <summary>
        /// Contains the individual configuration entries to be applied in order to patch a specific level's splines.
        /// start, end and brag positions.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<ConfigEntry> ConfigEntries;

        /// <summary>
        /// Parses an individual JSON exported file containing configuration entries.
        /// </summary>
        /// <param name="path">The location of the JSON file to be deserialized.</param>
        /// <returns>A serialized copy of the config object.</returns>
        public Config ParseConfigEntries(string path)
        {
            string allText = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Config>(allText);
        }

        /// <summary>
        /// Writes an individual config instance into a specified file path. 
        /// </summary>
        /// <param name="path">The individual file path to write to.</param>
        /// <param name="config">The config object instance to write.</param>
        public void WriteConfigEntries(string path, Config config)
        {
            string allText = JsonConvert.SerializeObject(config);
            File.WriteAllText(path, allText);
        }
    }
}