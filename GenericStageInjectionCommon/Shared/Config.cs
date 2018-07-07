using System;
using System.Collections.Generic;
using GenericStageInjectionCommon.Structs.Positions;
using Newtonsoft.Json;

namespace GenericStageInjectionCommon.Shared
{
    /// <summary>
    /// Represents an individual readable/writable configuration of starting and ending positions used by the Generic Stage Injection Mod DLLs
    /// and level editors such as Heroes Power Plant.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// A pointer to an array of pointers to spline headers that which is (the array) terminated by a null pointer.
        /// This is a parameter for the cdecl function responsible for loading splines. (e.g. push 7E2B40, call sub_439020)
        /// </summary>
        [JsonProperty(Required = Required.Default)] // Download NewtonSoft.Json from NuGet
        public int PushPointer;

        /// <summary>
        /// The memory address where the start positions for the current stage are stored.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int StartPointer;

        /// <summary>
        /// The memory address where the end positions for the current stage are stored.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int EndPointer;

        /// <summary>
        /// The memory address where the bragging positions for the current stage are stored.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public int BragPointer;

        /// <summary>
        /// Contains a set of generally 5 entries for each of the four teams and one unused team.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionStart> StartPositions;

        /// <summary>
        /// Contains a set of generally 5 entries for each of the four teams and one unused team.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionEnd> EndPositions;

        /// <summary>
        /// Contains a set of generally 5 entries for each of the four teams and one unused team.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public List<PositionBrag> BragPositions;

        //
        // I can't edit SplineHeader.cs (you probably need to save) but we can calculate the spline length from adding up the DistanceToNextVertex of each vertex in the lists of vertices.
        // I think I remember doing that even.
        // Oh, also, I just remembered.
        // Reloaded can also write classes to memory, not only structs, as long as some structure layout is defined.
        // i.e. It can write a class with attribute [StructLayout(LayoutKind.Sequential)] to memory.
        // Welcome back :3 (I was resting myself)

        // While we wait...
        // Time to plan how to cleanly generate splines from files and write them to memory :eyes:
        // 1. 
        // 2. 
        // 3. 
        // <Infinite>. Profit

        // we already have functions for that
        // we can just adapt them
        // yup, I'm more thinking about the order of operations. 
        // Add vertex list to memory, get location and put it in spline header, add header to memory,
        // get location and put it in pointer list, (repeat) write pointer list to memory, get location and write to push instruction.
        // Yup. I was just thinking if there were ways to shring code count :3

        // Nah :3
        // is that gonna be in this class?
    }

}

// how exactly will this work?
// We'll get the Newtonsoft.JSON serializer to automate it for us.
// Basically this class will replicate the config files we had in the old setup.
// See https://github.com/sewer56lol/Reloaded-Mod-Loader/blob/master/libReloaded-IO/IO/Config/LoaderConfig.cs

/* For Reference
HEROES_MOD_LOADER_LEVEL_CONFIG_FILE
LEVEL_FLAG=SeasideHill

PUSH_INSTRUCTION_POINTER=0x49E443
START_POSITION_POINTER=0x7C2FC8
END_POSITION_POINTER=0x7C45B8

SONIC_START_POSITIONX=103.47
SONIC_START_POSITIONY=7781.181
SONIC_START_POSITIONZ=-45.8315
SONIC_START_PITCH=32768
SONIC_START_MODE=0
SONIC_START_RUNNING=40

SONIC_END_POSITIONX=-9973.169
SONIC_END_POSITIONY=631
SONIC_END_POSITIONZ=-18400.4
SONIC_END_PITCH=62806

DARK_START_POSITIONX=103.47
DARK_START_POSITIONY=7781.181
DARK_START_POSITIONZ=-45.8315
DARK_START_PITCH=32768
DARK_START_MODE=0
DARK_START_RUNNING=40

DARK_END_POSITIONX=-9973.169
DARK_END_POSITIONY=631
DARK_END_POSITIONZ=-18400.4
DARK_END_PITCH=62806

ROSE_START_POSITIONX=103.47
ROSE_START_POSITIONY=7781.181
ROSE_START_POSITIONZ=-45.8315
ROSE_START_PITCH=32768
ROSE_START_MODE=0
ROSE_START_RUNNING=40

ROSE_END_POSITIONX=-9973.169
ROSE_END_POSITIONY=631
ROSE_END_POSITIONZ=-18400.4
ROSE_END_PITCH=62806

CHAOTIX_START_POSITIONX=103.47
CHAOTIX_START_POSITIONY=7781.181
CHAOTIX_START_POSITIONZ=-45.8315
CHAOTIX_START_PITCH=32768
CHAOTIX_START_MODE=0
CHAOTIX_START_RUNNING=40

CHAOTIX_END_POSITIONX=-9973.169
CHAOTIX_END_POSITIONY=631
CHAOTIX_END_POSITIONZ=-18400.4
CHAOTIX_END_PITCH=62806

FOREDIT_START_POSITIONX=103.47
FOREDIT_START_POSITIONY=7781.181
FOREDIT_START_POSITIONZ=-45.8315
FOREDIT_START_PITCH=32768
FOREDIT_START_MODE=0
FOREDIT_START_RUNNING=40

FOREDIT_END_POSITIONX=-9973.169
FOREDIT_END_POSITIONY=631
FOREDIT_END_POSITIONZ=-18400.4
FOREDIT_END_PITCH=62806
*/
