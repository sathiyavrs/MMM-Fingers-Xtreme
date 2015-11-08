using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlphaLevelGenerator : MonoBehaviour
{
    public GameObject GreenEnemy;
    public GameObject BoxEnemy;
    public GameObject CircleEnemy;
    public BoxCollider2D Bounds;
    public float BasicWait = 10f;
    public float BoxWait = 10f;
    public float CircleWait = 10f;

    public float InitialGameSpeed = 5f;
    public float GameAcceleration = 0f;
    public float BasicGeneration = 1.4f;
    public float BoxGeneration = 1.5f;
    public float CircleGeneration = 1.2f;
    public float TierTwoSpeed = 6f;
    public float TierThreeSpeed = 7f;

    private List<GameObject> _enemies;
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

    public void Start()
    {
        _enemies = new List<GameObject>();
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _waitDistanceCovered = 0;
        Globals.GameSpeed = InitialGameSpeed;
        _isGenerating = false;
        _hasStarted = false;
        _generationDistanceCovered = 0f;

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
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        if (Globals.GameSpeed == 0)
            Globals.GameSpeed = InitialGameSpeed;

        Globals.GameSpeed += GameAcceleration * Time.deltaTime * Globals.GlobalRatio;

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
        // Debug.Log(_itemGenerating);

        if (_itemGenerating == CustomResources.Generation.BasicGreenStraightThree)
        {
            // Debug.Log(_state + "1");
        }

        UpdateEnemies();
        UpdateGenerationInitialization();
    }

    private void UpdateGenerationInitialization()
    {
        if (Globals.GameSpeed > TierTwoSpeed)
        {
            InitializeGenerationListV2();
        }

        if(Globals.GameSpeed > TierThreeSpeed)
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

        if(InBasic(_itemGenerating))
        {
            if (_waitDistanceCovered > BasicWait)
            {
                _state = CustomResources.AlphaGeneratorState.Generating;
            }
            return;
        }

        if(InTierTwo(_itemGenerating))
        {
            if(_waitDistanceCovered > BoxWait)
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
        }

        _hasStarted = true;
    }

    private IEnumerator CircleOne()
    {
        var position = new Vector2(0, _spawnY);
        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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
        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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
            position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < BoxGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < BoxGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        enemy = (GameObject)Instantiate(GreenEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
        var enemy = (GameObject)Instantiate(BoxEnemy, position, new Quaternion());
        _enemies.Add(enemy);

        _generationDistanceCovered = 0;
        while (_generationDistanceCovered < 1.5f * BasicGeneration)
        {
            _generationDistanceCovered += Time.deltaTime * Globals.GlobalRatio * Globals.GameSpeed;
            yield return new WaitForFixedUpdate();
        }

        position.x = Random.Range(_leftEdge + 1, _rightEdge - 1);
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
