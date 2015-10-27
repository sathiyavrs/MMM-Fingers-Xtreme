public static class Globals
{
    public static bool GameOver
    {
        get
        {
            if (DebugMode)
                return false;

            return _gameOver;
        }
        set
        {
            _gameOver = value;
        }
    }

    public static bool SlowMotion { get; set; }
    public static int GameScore { get; set; }
    public static float SlowMotionRatio { get; set; }
    public static bool HasBeenInitialized { get; private set; }
    public static bool DebugMode { get; private set; }

    public static float GlobalRatio
    {
        get
        {
            return SlowMotion ? SlowMotionRatio : 1;
        }
    }


    private static bool _gameOver;

    public static void Initialize()
    {
        GameOver = false;
        SlowMotion = false;
        GameScore = 0;
        SlowMotionRatio = 0.3f;
        HasBeenInitialized = true;
        DebugMode = false;
    }
}

