/*

    Disclaimer : This class is full of properties and fields that are central to the working of the game. Look here hard before messing with the values.

    Also, this class is coded shabbily with no particular logical structure. My bad. Hope I'll come back and fix it one day.

    Okay, so some development notes here:

    * GameOver is the boolean that decides the basic game state.
    * SlowMotion is a boolean property that decides the slow motion mode of the game.
    * GameScore is a float that is later cast to an int to represent the score the user has earned so far.
    * 
    * SlowMotionRatio is the float that decides the Overall Game Speed, as ins
    * * The speed of all the blocks
    * * The speed with which the score increments
    * gets lowered by the SlowMotionRatio value. This is implemeted further in GlobalRatio.
    *
    * HasBeenInitialized is a boolean of no consequence.
    * 
    * DebugMode is an important boolean that sets the game in DebugMode. 
    * In this mode, the gameOver state can not be achieved.
    * Look at the definition of the GameOver property for more information.
    *
    * SlowMotionTimeData is a structure representing the various time data that deciedes whether
    * the game will go into slow motion.
    * MaxTime for slow motion, Increment and Decrement Rates, etc.
    * 
    * GameStarting is a boolean that suspends all computations when the game waits 
    * (for a default of five seconds = GameStartTime) while the player gets ready.
    * This is called the Starting Stage of the game.
    * 
    * GameStartTime is a float refering to the earlier mentioned property : GameStarting.
    *
    * ScoreIncrement is a float that represents the amount of score that the player has acheived 
    * since the last reward.
    * The player will need a minimum of 'RewardScore' to be rewarded with an extra life. 
    * This property will be discussed later.
    *
    * LivesIncremented is a boolean that's set to true when a life is incremented. 
    * This value is primarily meant for the GUIs. Do not mess with it unless you know what you're doing.
    * 
    * RewardScore is a float that represents the amount of score a player must earn to be rewarded.
    * The reward usually consists of an extra life. Works hand in hand with ScoreIncrement.
    *
    * StarCollected is a boolean that's set to true when the player collects a star.
    * This value is primarily meant for the GUIs. Do not mess with it unless you know what you're doing.
    *
    * StarReward represents the amount of reward the player has earned through collecting a star.
    * Equal to the PointsToAdd property on the CoinHandler.
    * This value is primarily meant for the GUIs. Do not mess with it unless you know what you're doing.
    *
    * GameSpeed is a float field that represents the Overall Game Speed.
    * This value represents the speed with which all the blocks move down the level.
    * Incremented Gradually by the LevelGenerator (Currently AlphaLevelGenerator).
    *
    * LivesRemaining is an int that represents the number of lives the player has remaining.
    * Increased within this class.
    * Decreased by External Factors.
    *
    * DangerSlowMotion is a boolean that represents the Danger Value for SlowMotion.
    * This value is by default 20 percent of the maximum SlowMotion time.
    * If the current SlowMotion time is lower than this value during regeneration,
    * the player cannot go to slow motion time.
    * 
    * Note that the Update method is called by either the LeapController or the HandController.
    * This is important. Do not not call this method by mistake.
    *
    * SlowMotionTimeData.PercentageRequiredToStart is the float value that works in tandem with DangerSlowMotion.
    * Modify in InitializeSlowMotion();
*/
using UnityEngine;

public static class Globals
{
    public static bool GameOver
    {
        get
        {
            // If DebugMode is turned on, GameOver is always false.
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
        RewardScore = 350;
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

