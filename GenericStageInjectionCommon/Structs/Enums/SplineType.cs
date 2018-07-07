namespace GenericStageInjectionCommon.Structs.Enums
{
    /// <summary>
    /// Represents an address (pointer) to a function used to handle the current spline.
    /// </summary>
    public enum SplineType : int
    {
        Null = 0,
        Loop = 0x433970,
        Rail = 0x4343F0,
        Ball = 0x434480
    }
}
