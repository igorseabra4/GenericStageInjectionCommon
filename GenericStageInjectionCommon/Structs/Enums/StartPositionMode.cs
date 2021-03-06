namespace GenericStageInjectionCommon.Structs.Enums
{
    /// <summary>
    /// Describes how the player starts off the stage, either on entry in challenge mode or once the opening cutscene cinematic (if in story) completes.
    /// </summary>
    public enum StartPositionMode : int
    {
        Normal = 0,
        Running = 1,
        Rail = 2
    }
}