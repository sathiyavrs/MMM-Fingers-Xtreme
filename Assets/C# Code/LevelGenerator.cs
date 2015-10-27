using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject Enemy;
    public BoxCollider2D Bounds;
    public float LevelSpeed;
    public float SpawnDistance;

    private float _speed;
    private List<GameObject> _enemies;
    private float _distanceCovered;
    private int _numberOfEnemiesToSpawn;
    private int _spawnRequestNumber;

    public void Start()
    {
        _enemies = new List<GameObject>();
        _distanceCovered = 0;
        _numberOfEnemiesToSpawn = 2;
        _spawnRequestNumber = 0;
        _speed = LevelSpeed;
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        UpdateDistance();
        UpdateSpawnNumber();
        UpdateEnemies();    
    }

    private void UpdateSpawnNumber()
    {
        if (_spawnRequestNumber >= 10)
        {
            _speed = 1.3f * LevelSpeed;
            _numberOfEnemiesToSpawn = 2;
        }

        if (_spawnRequestNumber >= 20)
        {
            _speed = 1.5f * LevelSpeed;
            _numberOfEnemiesToSpawn = 2;
        }


        if (_spawnRequestNumber >= 40)
        {
            _speed = 1.7f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }

        if (_spawnRequestNumber >= 60)
        {
            _speed = 2f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }


        if (_spawnRequestNumber >= 80)
        {
            _speed = 2.3f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }

        if (_spawnRequestNumber >= 100)
        {
            _speed = 2.5f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }

        if (_spawnRequestNumber >= 120)
        {
            _speed = 2.7f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }

        if (_spawnRequestNumber >= 140)
        {
            _speed = 3f * LevelSpeed;
            _numberOfEnemiesToSpawn = 3;
        }

        if (_spawnRequestNumber >= 180)
        {
            _speed = 3.2f * LevelSpeed;
            _numberOfEnemiesToSpawn = 4;
        }
        if (_spawnRequestNumber >= 200)
        {
            _speed = 3.5f * LevelSpeed;
            _numberOfEnemiesToSpawn = 4;
        }

    }

    private void UpdateDistance()
    {
        _distanceCovered += _speed * Time.deltaTime * Globals.GlobalRatio;
        if (_distanceCovered > SpawnDistance)
        {
            SpawnEnemies();
            _distanceCovered = 0;
            // Debug.Log(_enemies.Count);
        }
    }

    private void SpawnEnemies()
    {
        _spawnRequestNumber++;

        /*
            Code from BuzzSaws

            var effect = (GameObject)Instantiate(FireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            effect.transform.parent = transform;
        */

        var spawnY = Bounds.transform.position.y + (Bounds.offset.y + Bounds.size.y / 2) * Bounds.transform.localScale.y;
        var spawnMaxX = Bounds.transform.position.x + (Bounds.offset.x + Bounds.size.x / 2) * Bounds.transform.localScale.x;

        var spawnMinX = Bounds.transform.position.x + (Bounds.offset.x - Bounds.size.x / 2) * Bounds.transform.localScale.x;


        for (var i = 0; i < _numberOfEnemiesToSpawn; i++)
        {
            var position = new Vector2(Random.Range(spawnMinX, spawnMaxX), spawnY);
            var rotation = new Quaternion();

            var enemy = (GameObject)Instantiate(Enemy, position, rotation);
            _enemies.Add(enemy);
        }
    }

    private void UpdateEnemies()
    {
        var deltaY = -_speed * Globals.GlobalRatio * Time.deltaTime;

        foreach (GameObject enemy in _enemies)
        {
            enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + deltaY, enemy.transform.position.z);

            var vibrationHandler = enemy.GetComponent<VibrationHandler>();
            if (vibrationHandler == null)
                continue;

            vibrationHandler.SetCenterPosition(enemy.transform.position);
        }

        var randomOffsetValue = 10;
        var lowerY = Bounds.transform.position.y + (Bounds.offset.y - Bounds.size.y / 2) * Bounds.transform.localScale.y - randomOffsetValue;

        var _toDelete = new List<GameObject>();

        foreach(GameObject enemy in _enemies)
        {
            if(enemy.transform.position.y < lowerY)
            {
                _toDelete.Add(enemy);
            }
        }

        foreach (GameObject enemy in _toDelete)
        {
            _enemies.Remove(enemy);
            Destroy(enemy);
        }

        _toDelete.Clear();
    }
}