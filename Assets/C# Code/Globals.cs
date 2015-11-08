

using UnityEngine;

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
            if (!GameStarting)
                _gameOver = value;
        }
    }

    public static bool SlowMotion { get; private set; }
    public static float GameScore { get; set; }
    public static float SlowMotionRatio { get; set; }
    public static bool HasBeenInitialized { get; private set; }
    public static bool DebugMode { get; private set; }
    public static Vector2 Direction { get; private set; }

    public static CustomResources.SlowMotionData SlowMotionTimeData;

    public static bool GameStarting = true;

    public static float GlobalRatio
    {
        get
        {
            return SlowMotion ? SlowMotionRatio : 1;
        }
    }

    private static bool _gameOver;
    public static float GameStartTime = 5f;

    public static void Initialize()
    {
        GameOver = false;
        InitializeSlowMotion();
        SlowMotion = false;
        GameScore = 0;
        SlowMotionRatio = 0.3f;
        HasBeenInitialized = true;
        DebugMode = false;
        Direction = Vector2.down;
        GameSpeed = 0;
        LivesRemaining = 5;
        GameStarting = true;
        ScoreIncrement = 0;
        LivesIncremented = false;
        RewardScore = 100;
        StarCollected = false;
        StarReward = 0;
    }

    public static void InitializeSlowMotion()
    {
        SlowMotionTimeData = new CustomResources.SlowMotionData();
        SlowMotionTimeData.MaxTime = 10;
        SlowMotionTimeData.Current = 10;
        SlowMotionTimeData.IncrementRate = 1.2f;
        SlowMotionTimeData.DecrementRate = 3;
        SlowMotionTimeData.PercentageRequiredToStart = 0.2f;
    }

    public static void Update(float dt)
    {
        UpdateSlowMotion(dt);
        UpdateLivesRemaining();
    }

    public static float ScoreIncrement;
    public static bool LivesIncremented;
    public static float RewardScore;
    public static bool StarCollected;
    public static float StarReward;

    private static void UpdateLivesRemaining()
    {
        if(ScoreIncrement > RewardScore)
        {
            LivesRemaining++;
            LivesIncremented = true;
            ScoreIncrement = 0;
        }
    }

    private static void UpdateSlowMotion(float dt)
    {
        if (SlowMotion)
        {
            SlowMotionTimeData.Current -= SlowMotionTimeData.DecrementRate * dt;
        }
        else
        {
            SlowMotionTimeData.Current += SlowMotionTimeData.IncrementRate * dt;
        }

        if (SlowMotionTimeData.Current > SlowMotionTimeData.MaxTime)
        {
            SlowMotionTimeData.Current = SlowMotionTimeData.MaxTime;
        }

        if (SlowMotionTimeData.Current < 0)
        {
            SlowMotionTimeData.Current = 0;
        }

        if (SlowMotionTimeData.Current <= 0)
        {
            SlowMotion = false;
        }

    }

    public static float GameSpeed;
    public static int LivesRemaining = 5;


    public static void SetSlowMotionTrue()
    {
        if (SlowMotionTimeData.Current > (SlowMotionTimeData.PercentageRequiredToStart * SlowMotionTimeData.MaxTime))
            SlowMotion = true;
    }

    public static void SetSlowMotionFalse()
    {
        SlowMotion = false;
    }

    public static bool DangerSlowMotion
    {
        get
        {
            return (SlowMotionTimeData.Current <= (SlowMotionTimeData.PercentageRequiredToStart * SlowMotionTimeData.MaxTime));
        }
    }

}

