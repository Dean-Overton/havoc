using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private int currentLevel = 0;
    // private int currentWave = 0;
    // public Wave[] waves;
    public Level[] levels;
    private GameObject enemiesParent;

    public void BeginGame()
    {
        enemiesParent = GameObject.Find("_Enemies_");
        SpawnWave();
    }

    
    public void LevelComplete() {
        Debug.Log("Level " + currentLevel + " complete!");
        currentLevel++;
    }

    public void EnemyKilled(int enemyID) {
        Level _level = levels[currentLevel];
        Debug.Log("Enemy " + enemyID + " killed!");
        _level.waves[_level.getCurrentWave()].enemiesRemaining--;

        if (_level.waves[_level.getCurrentWave()].enemiesRemaining <= 0) {
            _level.IncrementWave();
            if (_level.getCurrentWave() < _level.waves.Length) {
                Debug.Log("Wave " + _level.getCurrentWave() + " complete!");
                SpawnWave();
            } else {
                LevelComplete();
            }
        }
    }

    private void SpawnWave() {
        Level _level = levels[currentLevel];
        // Debug.Log("Spawning wave " + _level.getCurrentWave());
        _level.waves[_level.getCurrentWave()].SetEnemiesRemaining(_level.waves[0].enemies.Length);
        for (int i = 0; i < _level.waves[_level.getCurrentWave()].enemies.Length; i++) {
            Instantiate(_level.waves[_level.getCurrentWave()].enemies[i], _level.waves[_level.getCurrentWave()].spawnPoints[i].position, _level.waves[_level.getCurrentWave()].spawnPoints[i].rotation, enemiesParent.transform);
        }

        Debug.Log("Number of enemies in wave " + _level.getCurrentWave() + ": " + _level.waves[_level.getCurrentWave()].enemiesRemaining);
    }

    void Update()
    {
        
    }
}
[System.Serializable]
public class Wave {
    public GameObject[] enemies;
    public Transform[] spawnPoints;

    [HideInInspector] public int enemiesRemaining;

    public void SetEnemiesRemaining(int numOfEnemiesRemaining) {
        this.enemiesRemaining = numOfEnemiesRemaining;
    }
}

[System.Serializable]
public class Level {
    public Wave[] waves;
    [HideInInspector] private int currentWave = 0;

    public void IncrementWave() {
        this.currentWave++;
    }

    public int getCurrentWave() {
        return this.currentWave;
    }

    // public int levelNumber;
    // public Level(int levelNumber, Wave[] waves) {
    //     this.levelNumber = levelNumber;
    //     this.waves = waves;
    // }
}
