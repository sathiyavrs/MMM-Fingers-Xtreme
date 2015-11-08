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

    public struct SlowMotionData
    {
        public float Current;
        public float MaxTime { get; set; }
        public float IncrementRate { get; set; }
        public float DecrementRate { get; set; }
        public float PercentageRequiredToStart { get; set; }
    }

    public enum AlphaGeneratorState
    {
        Generating,
        Waiting
    }

    public enum Generation
    {
        BasicGreenStraightTwo,
        BasicGreenStraightThree,
        BasicGreenThreeLine,
        BasicGreenTwoLineOne,
        BasicGreenTwoLineTwo,

        BlueTwo,
        BlueThree,
        BlueOne,
        
        GreenBlueOne,
        GreenBlueTwo,
        GreenBlueThree,

        CircleOne,
        CircleTwo,

        CircleBlueOne,
        
        CircleGreenOne
    }

}
