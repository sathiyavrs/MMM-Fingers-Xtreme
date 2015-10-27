public static class CustomResources
{
    public enum InterpolationDirection
    {
        Forward,
        Backward
    }

    public enum ObjectType
    {
        Friendly,
        Hostile,
        Neutral
    }

    public enum FlickerState
    {
        On,
        Off
    }

    public static void CheckForGlobalsInitialization()
    {
        if (!Globals.HasBeenInitialized)
            Globals.Initialize();
    } 
}
