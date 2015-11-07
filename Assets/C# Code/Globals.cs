

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
            _gameOver = value;
        }
    }

    public static bool SlowMotion { get; private set; }
    public static int GameScore { get; set; }
    public static float SlowMotionRatio { get; set; }
    public static bool HasBeenInitialized { get; private set; }
    public static bool DebugMode { get; private set; }
    public static Vector2 Direction { get; private set; }

    public static CustomResources.SlowMotionData SlowMotionTimeData;


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
        InitializeSlowMotion();
        SlowMotion = false;
        GameScore = 0;
        SlowMotionRatio = 0.3f;
        HasBeenInitialized = true;
        DebugMode = true;
        Direction = Vector2.down;
        GameSpeed = 0;
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

