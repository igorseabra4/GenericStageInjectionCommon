using GenericStageInjectionCommon.Structs.Positions;

namespace GenericStageInjectionCommon.Shared.Ingame
{
    /// <summary>
    /// Only to be used inside an injected DLL.
    /// Contains the pointers which point to the start of individual sections.
    /// </summary>
    public static unsafe class Pointers
    {
        public const int SINGLE_PLAYER_START_ENTRY_LENGTH = 27;
        public const int BOTH_PLAYER_END_ENTRY_LENGTH = 60;
        public const int MULTI_PLAYER_START_ENTRY_LENGTH = 23;
        public const int MULTI_PLAYER_BRAG_ENTRY_LENGTH = 21;

        /// <summary>
        /// Pointer to the current stage's singleplayer start positions for each team.
        /// </summary>
        public static SingleplayerStart* SinglePlayerStartPointer = (SingleplayerStart*)0x7C2FC8;

        /// <summary>
        /// Pointer to the current stage's multiplayer end positions for each team.
        /// </summary>
        public static SingleplayerEnd* BothPlayerEndPointer = (SingleplayerEnd*)0x7C45B8;

        /// <summary>
        /// Pointer to the current stage's multiplayer start positions for each team.
        /// </summary>
        public static MultiplayerStart* MultiPlayerStartPointer = (MultiplayerStart*) 0x7C5E18;

        /// <summary>
        /// Pointer to the current stage's multiplayer end positions for each team.
        /// </summary>
        public static MultiplayerBrag* MultiPlayerBragPointer = (MultiplayerBrag*)0x7C6380;
    }
}
