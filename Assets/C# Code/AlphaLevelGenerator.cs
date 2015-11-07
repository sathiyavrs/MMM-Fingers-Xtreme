using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlphaLevelGenerator : MonoBehaviour
{
    public GameObject GreenEnemy;
    public GameObject BoxEnemy;
    public GameObject CircleEnemy;
    public BoxCollider2D Bounds;
    public float WaitDistance = 10f;
    public float InitialGameSpeed = 5f;
    public float GameAcceleration = 0f;

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

    public void Start()
    {
        _enemies = new List<GameObject>();
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _waitDistanceCovered = 0;
        Globals.GameSpeed = InitialGameSpeed;
        _isGenerating = false;
        _hasStarted = false;

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


    public void Update()
    {
        InitialGameSpeed += GameAcceleration * Time.deltaTime * Globals.GlobalRatio;
        
        switch (_state)
        {
            case CustomResources.AlphaGeneratorState.Waiting:
                UpdateWaiting();
                break;

            case CustomResources.AlphaGeneratorState.Generating:
                UpdateGenerating();
                break;
        }

        UpdateEnemies();
    }

    private void UpdateEnemies()
    {
        var toDelete = new List<GameObject>();
        var deltaY = Time.deltaTime * InitialGameSpeed * Globals.GlobalRatio * -1;

        foreach(GameObject enemy in _enemies)
        {
            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + deltaY, enemy.transform.position.z);

            var vibrationHandler = enemy.GetComponent<VibrationHandler>();
            if(vibrationHandler != null)
            {
                vibrationHandler.SetCenterPosition(enemy.transform.position);
            }

            if(enemy.transform.position.y < _destroyY)
            {
                toDelete.Add(enemy);
            }
        }

        foreach(GameObject enemy in toDelete)
        {
            _enemies.Remove(enemy);
        }
    }

    private void UpdateWaiting()
    {
        _waitDistanceCovered += Time.deltaTime * InitialGameSpeed;
        if (_waitDistanceCovered > WaitDistance)
        {
            _state = CustomResources.AlphaGeneratorState.Generating;
        }

    }

    private void UpdateGenerating()
    {
        if (!_isGenerating)
        {
            _isGenerating = true;
            // Basic Line Count = 5;

            var random = (int)Random.Range(0, _generationList.Count);
            _itemGenerating = _generationList[random];
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
        }
        _hasStarted = true;
    }

    public IEnumerator BasicGreenStraightTwo()
    {
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _hasStarted = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        yield break;
    }

    public IEnumerator BasicGreenStraightThree()
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

    public IEnumerator BasicGreenThreeLine()
    {
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        yield return new WaitForSeconds(0.5f);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        yield return new WaitForSeconds(0.5f);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    public IEnumerator BasicGreenTwoLineOne()
    {
        var spawnPositionOne = new Vector2(0, _spawnY);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        var enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        yield return new WaitForSeconds(0.5f);

        spawnPositionOne.x = Random.Range(_leftEdge, _rightEdge);
        enemy = (GameObject)Instantiate(GreenEnemy, spawnPositionOne, new Quaternion());
        _enemies.Add(enemy);

        _isGenerating = false;
        _state = CustomResources.AlphaGeneratorState.Waiting;
        _hasStarted = false;
        yield break;
    }

    public IEnumerator BasicGreenTwoLineTwo()
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

        yield return new WaitForSeconds(0.5f);

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
