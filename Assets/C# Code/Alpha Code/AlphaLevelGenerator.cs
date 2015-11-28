/*
    
    Documentation:

    Let me first give a basic idea of how this Generator works:

    The generator has two stages, the Waiting stage, and the Generating stage.
    This is governed by the _state private field of type 'CustomResources.AlphaGeneratorState'.

    Note that the downward movement of enemies is not governed by these stages. 
    That happens automatically all the time.

    Let me discuss the 'Generating' state first. 

    The aim of this generator is to generate 'Structures of Blocks'. 
    Each structure is a set of blocks the player must navigate through to survive.
    Each structure is different from each other.

    To do that, this script generates a 'CustomResources.Generation' called _itemGenerating.
    This value uniquely identifies one of several possible structure designed by the developer.

    After identifying one possible value for _itemGenerating, the script calls a initiates a CoRoutine.
    This Coroutine will essentially halt the execution of the Update() method until it is finished.

    As soon as the CoRoutine yields, it sets a bunch of variables telling the Script to go to Waiting stage.
    
    Now, the waiting stage simply does nothing but move all the enemies forward, until a certain distance is covered.
    This distance is based on the value of _itemGenerating, ie, the more complex the item generated, the more the waiting time.
    
    As soon as the waiting's done, the game moves back to the Generating Stage.

    ***

    Now, I'll be discussing all the public properties I've defined in my script. 
    I'll go over them one by one.

    * GreenEnemy - Coins represent prefabs for those items.
    * Bounds represents the bounds of the map. This value is mainly used for Computations for Level Generation.
    * Player represents the player object the game uses.
    
    Before I describe the next three properties, let me describe the types of structures I've defined.

    There are three types: Basic, TierOne, TierTwo.

    Basic represents the basic combinations of the GreenEnemy Type alone. This enemy type doesn't move around much.
    It vibrates a bit and mainly just moves down. The basic enemy type, if you will.

    TierOne includes the BoxEnemy. This enemy defines a BoxCollider2D object within it as a bounds, and
    generates points within it as target points. It then moves towards those target points. Once it reaches
    a target point, it moves to square one.
    TierOne represents several combinations including the Basic Green Enemy type and the BoxEnemy type.

    TierTwo includes the CircleEnemy. This enemy moves in a spiral around a min and Max Radius values around a center.
    They can move either in clockwise or anticlockwise directions.
    Naturally, we simply move the center down.
    TierTwo inclues combinations of all enemy types.

    * BasicWait represents the distance that must be covered when a TierOne structure had just been generated.
    * This distance will be covered in the Waiting State of this script.
    *
    * BoxWait represents the distance that must be covered when a TierTwo structure had just been generated.
    * This distance will be covered in the Waiting State of this script.
    *
    * CircleWait represents the distance that must be covered when a TierThree structure had just been generated.
    * This distance will be covered in the Waiting State of this script.

    You can increase these values to make the game a bit easier.
    
    * InitialGameSpeed represents the Speed which is set to 'Globals.GameSpeed'. Look that up for more info.
    * MaxGameSpeed represents the max value 'Globals.GameSpeed' can achieve.
    * GameAcceleration represents the acceleration value for 'Globals.GameSpeed'

    The next three values (Basic, Box and Circle Generation) deal with the internals of working within 
    the structure generation. Do not change them unless you're really sure what you're doing.

    TODO: Document the meanings of Basic, Box and Circle Generation.

    * TierTwoSpeed represents the value of Globals.GameSpeed at which the game starts generating TierTwo enemies.
    * TierThreeSpeed represents the value of Globals.GameSpeed at which the game starts generating TierThree enemies.
    
    Okay. A quick heads-up of how coins work.
    They essentially add to the number of points the player gets, through the PointsToAdd field on the CoinHandler 
    script. But, on contact with an enemy block, they're destroyed.
    I did this to allow me to generate coins and enemies separately to ease my design process.
    A mistake I'll probably regret in the future.

    * CoinDistance represents the distance that has to be covered by the GameSpeed for coins to be generated.
    
    The rate at which idle scoring is incremented is based on the player's Y value. 
    If the player is situated at a higher y value, the scoring is faster.

    * ScoreMaxIncrement represents the value with which the score increases if the player's at the top of the screen.
    * ScoreMinIncrement represents the value with which the score increases if the player's at the bottom of the screen.

    This should provide all the documentation needed to modify the LevelGenerator's parameters.
*/


using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlphaLevelGenerator : MonoBehaviour
{
    public GameObject GreenEnemy;
    public GameObject BoxEnemy;
    public GameObject CircleEnemy;
    public GameObject Coins;
    public BoxCollider2D Bounds;
    public GameObject Player;

    public float BasicWait = 10f;
    public float BoxWait = 10f;
    public float CircleWait = 10f;

    public float InitialGameSpeed = 5f;
    public float MaxGameSpeed = 7f;
    public float GameAcceleration = 0f;

    public float BasicGeneration = 1.4f;
    public float BoxGeneration = 1.5f;
    public float CircleGeneration = 1.2f;

    public float TierTwoSpeed = 6f;
    public float TierThreeSpeed = 7f;

    public float CoinDistance = 6f;

    public float ScoreMaxIncrement = 10f;
    public float ScoreMinIncrement = 5f;

    private List<GameObject> _enemies;
    private List<GameObject> _coins;
    private CustomResources.AlphaGeneratorState _state;
    private CustomResources.Generation _itemGenerating;
    private float _waitDistanceCovered;
    private bool _isGenerating;
    private List<CustomResources.Generation> _generationList;
    private float _spawnY;
    private float _destroyY;
    private float _leftEdge;
    private float _rightEdge;
    private bool _hasStarted;
    private float _generationDistanceCovered;
    private float _coinDistanceCovered;
    private float _gameStartTimeLeft;

    private float _initialScoreMin;
    private float _initialScoreMax;

    public void Start()
    {
        _enemies = new List<GameObject>();
        _coins = new List<GameObject>();
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _waitDistanceCovered = 0;
        Globals.GameSpeed = InitialGameSpeed;
        _isGenerating = false;
        _hasStarted = false;
        _generationDistanceCovered = 0f;
        _coinDistanceCovered = 0f;
        _gameStartTimeLeft = Globals.GameStartTime;
        _initialScoreMax = ScoreMaxIncrement;
        _initialScoreMin = ScoreMinIncrement;

        InitializeGenerationList();
        CalculateSpawnY();
        CalculateDestroyY();
        CalculateEdges();
    }

    private void CalculateEdges()
    {
        var leftEdge = Bounds.transform.position.x + (Bounds.offset.x - Bounds.size.x / 2) * Bounds.transform.localScale.x;

        _leftEdge = leftEdge + 1;

        var rightEdge = Bounds.transform.position.x + (Bounds.offset.x + Bounds.size.x / 2) * Bounds.transform.localScale.x;

        _rightEdge = rightEdge - 1;
    }

    private void CalculateDestroyY()
    {
        var boundsBottom = Bounds.transform.position.y + (Bounds.offset.y - Bounds.size.y / 2) * Bounds.transform.localScale.y;

        _destroyY = boundsBottom - 2;
    }

    private void CalculateSpawnY()
    {
        var boundsTop = Bounds.transform.position.y + (Bounds.offset.y + Bounds.size.y / 2) * Bounds.transform.localScale.y;

        _spawnY = boundsTop + 2;
    }


    private void InitializeGenerationList()
    {
        _generationList = new List<CustomResources.Generation>();

        _generationList.Add(CustomResources.Generation.BasicGreenStraightTwo);
        _generationList.Add(CustomResources.Generation.BasicGreenStraightThree);
        _generationList.Add(CustomResources.Generation.BasicGreenThreeLine);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineOne);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineTwo);

    }

    private void InitializeGenerationListV2()
    {
        _generationList = new List<CustomResources.Generation>();

        _generationList.Add(CustomResources.Generation.BasicGreenStraightTwo);
        _generationList.Add(CustomResources.Generation.BasicGreenStraightThree);
        _generationList.Add(CustomResources.Generation.BasicGreenThreeLine);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineOne);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineTwo);

        _generationList.Add(CustomResources.Generation.BlueOne);
        _generationList.Add(CustomResources.Generation.BlueTwo);
        _generationList.Add(CustomResources.Generation.BlueThree);

        _generationList.Add(CustomResources.Generation.GreenBlueOne);
        _generationList.Add(CustomResources.Generation.GreenBlueTwo);
        _generationList.Add(CustomResources.Generation.GreenBlueThree);
    }

    private void InitializeGenerationListV3()
    {
        _generationList = new List<CustomResources.Generation>();

        _generationList.Add(CustomResources.Generation.BasicGreenStraightTwo);
        _generationList.Add(CustomResources.Generation.BasicGreenStraightThree);
        _generationList.Add(CustomResources.Generation.BasicGreenThreeLine);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineOne);
        _generationList.Add(CustomResources.Generation.BasicGreenTwoLineTwo);

        _generationList.Add(CustomResources.Generation.BlueOne);
        _generationList.Add(CustomResources.Generation.BlueTwo);
        _generationList.Add(CustomResources.Generation.BlueThree);

        _generationList.Add(CustomResources.Generation.GreenBlueOne);
        _generationList.Add(CustomResources.Generation.GreenBlueTwo);
        _generationList.Add(CustomResources.Generation.GreenBlueThree);

        _generationList.Add(CustomResources.Generation.CircleOne);
        _generationList.Add(CustomResources.Generation.CircleTwo);
        _generationList.Add(CustomResources.Generation.CircleBlueOne);
        _generationList.Add(CustomResources.Generation.CircleGreenOne);
    }

    private void GameStart()
    {
        _gameStartTimeLeft -= Time.deltaTime * Globals.GlobalRatio;
        if (_gameStartTimeLeft <= 0)
        {
            Globals.GameStarting = false;
        }
    }

    private void RestartListener()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (Globals.GameOver)
            {
                Restart();
            }

        }
    }

    private void Restart()
    {
        var toDelete = new List<GameObject>();

        foreach (GameObject enemy in _enemies)
        {
            toDelete.Add(enemy);
        }

        foreach (GameObject coin in _coins)
            toDelete.Add(coin);

        foreach (GameObject obj in toDelete)
        {
            Destroy(obj);
        }

        ScoreMinIncrement = _initialScoreMin;
        ScoreMaxIncrement = _initialScoreMax;

        Start();
        Globals.Initialize();
        Globals.GameStarting = true;
    }

    public void Update()
    {
        RestartListener();

        if (Globals.GameOver)
            return;

        if (Globals.GameStarting)
        {
            GameStart();
            return;
        }

        if (Globals.GameSpeed == 0)
            Globals.GameSpeed = InitialGameSpeed;

        Globals.GameSpeed += GameAcceleration * Time.deltaTime * Globals.GlobalRatio;
        if(Globals.GameSpeed > MaxGameSpeed)
        {
            Globals.GameSpeed = MaxGameSpeed;
        }

        switch (_state)
        {
            case CustomResources.AlphaGeneratorState.Waiting:
                _generationDistanceCovered = 0f;
                UpdateWaiting();
                break;

            case CustomResources.AlphaGeneratorState.Generating:
                _waitDistanceCovered = 0f;
                UpdateGenerating();
                break;
        }

        UpdateEnemies();
        UpdateGenerationInitialization();
        UpdateCoins();
        UpdateScore();
    }

    private void UpdateScore()
    {
        ScoreMinIncrement += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed / 100;
        ScoreMaxIncrement += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed / 100;


        var playerY = Player.transform.position.y;
        var normalizedPlayerY = (playerY - _destroyY) / (_spawnY - _destroyY);
        var scoreIncrement = normalizedPlayerY * (ScoreMaxIncrement - ScoreMinIncrement) + ScoreMinIncrement;

        // Debug.Log(_spawnY - _destroyY);

        var deltaScore = scoreIncrement * Time.deltaTime * Globals.GlobalRatio;
        Globals.GameScore += deltaScore;
        Globals.ScoreIncrement += deltaScore;
        // Debug.Log((int)Globals.GameScore);
    }

    private void UpdateCoins()
    {
        UpdateAddCoins();
        UpdateMoveCoins();
        UpdateDeleteCoins();
    }

    private void UpdateMoveCoins()
    {
        var deltaY = -1 * Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
        var toDelete = new List<GameObject>();

        foreach (GameObject coin in _coins)
        {
            if (coin == null)
            {
                toDelete.Add(coin);
                continue;
            }

            coin.transform.position = new Vector3(coin.transform.position.x, coin.transform.position.y + deltaY, coin.transform.position.z);
        }

        foreach (GameObject coin in toDelete)
        {
            _coins.Remove(coin);
        }
    }

    private void UpdateAddCoins()
    {
        _coinDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;

        if (_coinDistanceCovered < CoinDistance)
            return;

        _coinDistanceCovered = 0;

        var number = (int)Random.Range(1, 4);
        var position = new Vector2(0, _spawnY);

        while (number-- > 0)
        {
            position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
            var coin = (GameObject)Instantiate(Coins, position, new Quaternion());
            _coins.Add(coin);
        }
    }

    private void UpdateDeleteCoins()
    {
        var toDelete = new List<GameObject>();

        foreach (GameObject coin in _coins)
        {
            if (coin.transform.position.y < _destroyY)
            {
                toDelete.Add(coin);
            }
        }

        foreach (GameObject coin in toDelete)
        {
            _coins.Remove(coin);
            Destroy(coin);
        }
    }



    private void UpdateGenerationInitialization()
    {
        if (Globals.GameSpeed > TierTwoSpeed)
        {
            InitializeGenerationListV2();
        }

        if (Globals.GameSpeed > TierThreeSpeed)
        {
            InitializeGenerationListV3();
        }
    }

    private void UpdateEnemies()
    {


        var toDelete = new List<GameObject>();
        var deltaY = Time.deltaTime * Globals.GameSpeed * Globals.GlobalRatio * -1;

        foreach (GameObject enemy in _enemies)
        {
            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + deltaY, enemy.transform.position.z);

            var vibrationHandler = enemy.GetComponent<VibrationHandler>();
            if (vibrationHandler != null)
            {
                vibrationHandler.SetCenterPosition(enemy.transform.position);
            }

            if (enemy.transform.position.y < _destroyY)
            {
                toDelete.Add(enemy);
            }
        }

        foreach (GameObject enemy in toDelete)
        {
            _enemies.Remove(enemy);
            Destroy(enemy);
        }
    }

    private bool InBasic(CustomResources.Generation state)
    {
        if (state == CustomResources.Generation.BasicGreenStraightThree || state == CustomResources.Generation.BasicGreenStraightTwo || state == CustomResources.Generation.BasicGreenThreeLine || state == CustomResources.Generation.BasicGreenTwoLineOne || state == CustomResources.Generation.BasicGreenTwoLineTwo)
        {
            return true;
        }

        return false;
    }

    private bool InTierTwo(CustomResources.Generation state)
    {
        if (state == CustomResources.Generation.BlueOne || state == CustomResources.Generation.BlueThree || state == CustomResources.Generation.BlueTwo || state == CustomResources.Generation.GreenBlueOne || state == CustomResources.Generation.GreenBlueThree || state == CustomResources.Generation.GreenBlueTwo)
            return true;

        return false;
    }

    private void UpdateWaiting()
    {
        _waitDistanceCovered += Time.deltaTime * Globals.GameSpeed;

        if (InBasic(_itemGenerating))
        {
            if (_waitDistanceCovered > BasicWait)
            {
                _state = CustomResources.AlphaGeneratorState.Generating;
            }
            return;
        }

        if (InTierTwo(_itemGenerating))
        {
            if (_waitDistanceCovered > BoxWait)
            {
                _state = CustomResources.AlphaGeneratorState.Generating;
            }

            return;
        }

        if (_waitDistanceCovered > CircleWait)
            _state = CustomResources.AlphaGeneratorState.Generating;
    }

    private void UpdateGenerating()
    {
        if (!_isGenerating)
        {
            _isGenerating = true;
            // Basic Line Count = 5;

            var random = (int)Random.Range(0, _generationList.Count);
            _itemGenerating = _generationList[random];
            _hasStarted = false;
        }

        if (_hasStarted)
            return;

        switch (_itemGenerating)
        {
            case CustomResources.Generation.BasicGreenStraightTwo:
                StartCoroutine(BasicGreenStraightTwo());
                break;

            case CustomResources.Generation.BasicGreenStraightThree:
                StartCoroutine(BasicGreenStraightThree());
                break;

            case CustomResources.Generation.BasicGreenThreeLine:
                StartCoroutine(BasicGreenThreeLine());
                break;

            case CustomResources.Generation.BasicGreenTwoLineOne:
                StartCoroutine(BasicGreenTwoLineOne());
                break;

            case CustomResources.Generation.BasicGreenTwoLineTwo:
                StartCoroutine(BasicGreenTwoLineTwo());
                break;

            case CustomResources.Generation.BlueOne:
                StartCoroutine(Blue(1));
                break;

            case CustomResources.Generation.BlueTwo:
                StartCoroutine(Blue(2));
                break;

            case CustomResources.Generation.BlueThree:
                StartCoroutine(Blue(2));
                break;

            case CustomResources.Generation.GreenBlueOne:
                StartCoroutine(GreenBlueOne());
                break;

            case CustomResources.Generation.GreenBlueTwo:
                StartCoroutine(GreenBlueOne());
                break;

            case CustomResources.Generation.GreenBlueThree:
                StartCoroutine(GreenBlueThree());
                break;

            case CustomResources.Generation.CircleOne:
                StartCoroutine(CircleOne());
                break;

            case CustomResources.Generation.CircleTwo:
                StartCoroutine(CircleTwo());
                break;

            case CustomResources.Generation.CircleBlueOne:
                StartCoroutine(CircleBlueOne());
                break;

            case CustomResources.Generation.CircleGreenOne:
                StartCoroutine(CircleGreenOne());
                break;
        }

        _hasStarted = true;
    }

    private IEnumerator CircleGreenOne()
    {
        var position = new Vector2(0, _spawnY);
        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(CircleEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        var circle = enemy.GetComponent<MagicCircleHandler>();
        circle.Clockwise = (Random.Range(0, 1) > 0.5f) ? true : false;

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    private IEnumerator CircleBlueOne()
    {
        var position = new Vector2(0, _spawnY);
        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(CircleEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        var circle = enemy.GetComponent<MagicCircleHandler>();
        circle.Clockwise = (Random.Range(0, 1) > 0.5f) ? true : false;

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    private IEnumerator CircleOne()
    {
        var position = new Vector2(0, _spawnY);
        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(CircleEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    private IEnumerator CircleTwo()
    {
        var position = new Vector2(0, _spawnY);
        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(CircleEnemy, position, new Quaternion());

        var circle = enemy.GetComponent<MagicCircleHandler>();
        circle.Clockwise = true;
        _enemies.Add(enemy);

        enemy = (GameObject)Instantiate(CircleEnemy, position, new Quaternion());
        circle = enemy.GetComponent<MagicCircleHandler>();
        circle.Clockwise = false;
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    private IEnumerator Blue(int count)
    {
        var position = new Vector2(0, _spawnY);

        while (count-- > 0)
        {
            position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
            var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());

            _enemies.Add(enemy);
        }

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator GreenBlueThree()
    {
        var position = new Vector2(0, _spawnY);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < BoxGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }


    private IEnumerator GreenBlueTwo()
    {
        var position = new Vector2(0, _spawnY);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < BoxGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator GreenBlueOne()
    {
        var position = new Vector2(0, _spawnY);

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < 1.5f * BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 0.5f, _rightEdge - 0.5f);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator BasicGreenStraightTwo()
    {
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator BasicGreenStraightThree()
    {
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator BasicGreenThreeLine()
    {

        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        while (_generationDistanceCovered < BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        _generationDistanceCovered = 0f;

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        while (_generationDistanceCovered < BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        _generationDistanceCovered = 0;

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;


        yield break;
    }

    private IEnumerator BasicGreenTwoLineOne()
    {
        _generationDistanceCovered = 0;
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        while (_generationDistanceCovered < BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        _generationDistanceCovered = 0;

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }

    private IEnumerator BasicGreenTwoLineTwo()
    {
        _generationDistanceCovered = 0;
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        while (_generationDistanceCovered < BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        _generationDistanceCovered = 0;

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;

        yield break;
    }


    // Hi Kamlesh
    // 
    // Hope your doing good, man!
    // Just wanted to wish you a happy birthday!
    // Think about getting back here sometime?
    // A meet would be nice!
    // Cheers!
}
