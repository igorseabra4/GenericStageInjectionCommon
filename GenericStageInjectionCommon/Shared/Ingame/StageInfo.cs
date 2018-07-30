using GenericStageInjectionCommon.Shared.Enums;
using GenericStageInjectionCommon.Structs.Positions;

namespace GenericStageInjectionCommon.Shared.Ingame
{
    /// <summary>
    /// Defines an individual stage configuration that defines 
    /// the individual properties of an individual action stage.
    /// The individual struct entries are available if the struct value member is non-zero.
    /// </summary>
    public unsafe struct StageInfo
    {
        /// <summary>
        /// Pointer to a parameter (another pointer) to be passed to function sub_439020 responsible for loading
        /// a given spline master table onto a stage.
        /// The final target is an array of pointers to spline headers terminated by a null pointer.
        /// </summary>
        // public Spline** SplinePushAddress;
        // Not removed in csae of possible re-usage.

        /// <summary>
        /// Pointer to the current stage's singleplayer start positions for each team.
        /// </summary>
        public SingleplayerStart* SinglePlayerStartPointer;

        /// <summary>
        /// Pointer to the current stage's multiplayer end positions for each team.
        /// </summary>
        public SingleplayerEnd* BothPlayerEndPointer;

        /// <summary>
        /// Pointer to the current stage's multiplayer start positions for each team.
        /// </summary>
        public MultiplayerStart* MultiPlayerStartPointer;

        /// <summary>
        /// Pointer to the current stage's multiplayer end positions for each team.
        /// </summary>
        public MultiplayerBrag* MultiPlayerBragPointer;

        /// <summary>
        /// Obtains the pointers for a specific stage's start/ending/bragging positions to override with the custom
        /// defined ones.
        /// </summary>
        /// <param name="stageId">The individual stage index to get the information for.</param>
        public static StageInfo GetStageInfo(StageID stageId)
        {
            // Categorize the individual stage and decide if to take 1P or 2P path.
            StageTags.StageTag stageTags = StageTags.CategorizeStage((int)stageId);
            StageInfo stageInfo = new StageInfo();

            // Check if 2 player to perform or 1 player.
            if (stageTags.HasFlag(StageTags.StageTag.TwoPlayer))
            {
                #region Get Multiplayer Start
                for (int x = 0; x < Pointers.MULTI_PLAYER_START_ENTRY_LENGTH; x++)
                {
                    if (Pointers.MultiPlayerStartPointer[x].StageID == stageId)
                    {
                        stageInfo.MultiPlayerStartPointer = &Pointers.MultiPlayerStartPointer[x];
                        break;
                    }
                }
                #endregion

                #region Get Multiplayer Brag
                for (int x = 0; x < Pointers.MULTI_PLAYER_BRAG_ENTRY_LENGTH; x++)
                {
                    if (Pointers.MultiPlayerBragPointer[x].StageID == stageId)
                    {
                        stageInfo.MultiPlayerBragPointer = &Pointers.MultiPlayerBragPointer[x];
                        break;
                    }
                }
                #endregion
            }
            else
            {
                #region Get Singleplayer Start
                for (int x = 0; x < Pointers.SINGLE_PLAYER_START_ENTRY_LENGTH; x++)
                {
                    if (Pointers.SinglePlayerStartPointer[x].StageID == stageId)
                    {
                        stageInfo.SinglePlayerStartPointer = &Pointers.SinglePlayerStartPointer[x];
                        break;
                    }
                }
                #endregion
            }

            #region Find Ending Position
            for (int x = 0; x < Pointers.BOTH_PLAYER_END_ENTRY_LENGTH; x++)
            {
                if (Pointers.BothPlayerEndPointer[x].StageID == stageId)
                {
                    stageInfo.BothPlayerEndPointer = &Pointers.BothPlayerEndPointer[x];
                    break;
                }
            }
            #endregion

            return stageInfo;
        }

        /// <summary>
        /// Applies an individual ConfigEntry configuration to a supplied stageInfo structure allowing for the writing of
        /// custom set level start and end locations.
        /// </summary>
        /// <param name="stageInfo">Contains the pointers defining the location of individual entries to override.</param>
        /// <param name="configEntry">Contains the individual spawn positions to be overwritten.</param>
        public static void ApplyConfig(StageInfo stageInfo, ConfigEntry configEntry)
        {
            // Categorize the individual stage and decide if to take 1P or 2P path.
            StageTags.StageTag stageTags = StageTags.CategorizeStage((int)configEntry.StageId);

            // Check if 2P
            if (stageTags.HasFlag(StageTags.StageTag.TwoPlayer))
            {
                // Start and Ending Positions have 2 Entries for 1P & 2P
                // Bragging positions are 4, one for each team.
                #region Multiplayer Brag
                if (stageInfo.MultiPlayerBragPointer != null)
                {
                    (*stageInfo.MultiPlayerBragPointer).Sonic   = configEntry.BragPositions[(int)Teams.Sonic];
                    (*stageInfo.MultiPlayerBragPointer).Dark    = configEntry.BragPositions[(int)Teams.Dark];
                    (*stageInfo.MultiPlayerBragPointer).Rose    = configEntry.BragPositions[(int)Teams.Rose];
                    (*stageInfo.MultiPlayerBragPointer).Chaotix = configEntry.BragPositions[(int)Teams.Chaotix];
                }
                #endregion Multiplayer Brag

                #region Multiplayer Start
                if (stageInfo.MultiPlayerStartPointer != null)
                {
                    (*stageInfo.MultiPlayerStartPointer).Player1Start = configEntry.StartPositions[0];
                    (*stageInfo.MultiPlayerStartPointer).Player2Start = configEntry.StartPositions[1];
                }
                #endregion Multiplayer Start
            }
            else
            {
                // Start and Ending Positions have 5 entries for each team.
                #region Singleplayer Start
                if (stageInfo.SinglePlayerStartPointer != null)
                {
                    (*stageInfo.SinglePlayerStartPointer).SonicStart    = configEntry.StartPositions[(int)Teams.Sonic];
                    (*stageInfo.SinglePlayerStartPointer).DarkStart     = configEntry.StartPositions[(int)Teams.Dark];
                    (*stageInfo.SinglePlayerStartPointer).RoseStart     = configEntry.StartPositions[(int)Teams.Rose];
                    (*stageInfo.SinglePlayerStartPointer).ChaotixStart  = configEntry.StartPositions[(int)Teams.Chaotix];
                    (*stageInfo.SinglePlayerStartPointer).ForeditStart  = configEntry.StartPositions[(int)Teams.ForEDIT];
                }
                #endregion Singleplayer Start
            }

            // Ending format is the same for both 1P and 2P
            #region Both Player End
            if (stageInfo.BothPlayerEndPointer != null)
            {
                (*stageInfo.BothPlayerEndPointer).SonicEnd = configEntry.EndPositions[(int)Teams.Sonic];
                (*stageInfo.BothPlayerEndPointer).DarkEnd = configEntry.EndPositions[(int)Teams.Dark];
                (*stageInfo.BothPlayerEndPointer).RoseEnd = configEntry.EndPositions[(int)Teams.Rose];
                (*stageInfo.BothPlayerEndPointer).ChaotixEnd = configEntry.EndPositions[(int)Teams.Chaotix];
                (*stageInfo.BothPlayerEndPointer).ForeditEnd = configEntry.EndPositions[(int)Teams.ForEDIT];
            }
            #endregion
        }
    }
}
