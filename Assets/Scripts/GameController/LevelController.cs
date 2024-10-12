using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private int currentLevel = 0;
    public Level[] levels;
    private Level[] initialLevels;
    private GameObject enemiesParent;
    private GameObject bridgesParent;
    private GameObject BarriersParent;

    void Start()
    {
        currentLevel = 0;
        initialLevels = levels;
    }
    void Awake()
    {
        enemiesParent = GameObject.Find("_Enemies_");
        bridgesParent = GameObject.Find("_Bridges_");
        BarriersParent = GameObject.Find("_Removable_Barriers_");

        if (enemiesParent == null)
        {
            Debug.LogError("Enemies parent object not found!");
        }

        if (bridgesParent == null)
        {
            Debug.LogError("Bridges parent object not found!");
        }
        if (BarriersParent == null)
        {
            Debug.LogError("Barriers parent object not found!");
        }
    }
    public void BeginGame()
    {
        levels = initialLevels;
        currentLevel = 0;
        SpawnWave();
    }

    public void StartNewLevel(int _levelID) {
        if (_levelID == currentLevel) {
            SpawnWave(); // INVOKE
        } else {
            Debug.Log("Level " + _levelID + " not yet unlocked!");
        }
    }

    
    public void LevelComplete() {
        Debug.Log("Level " + currentLevel + " complete!");
        currentLevel++;

        foreach (Transform bridge in bridgesParent.transform) {
            if (bridge.gameObject.name == "Bridge" + currentLevel) {
                bridge.gameObject.SetActive(true);
            } else {
                bridge.gameObject.SetActive(false);
            }
        }
        foreach (Transform barrier in BarriersParent.transform)
        {
            if (barrier.gameObject.name == "barrier" + currentLevel)
            {
                barrier.gameObject.SetActive(false);
            }
            else
            {
                barrier.gameObject.SetActive(true);
            }
        }
        if (currentLevel >= levels.Length) {
            EventManager.BroadcastGameComplete();
            return;
        }
    }

    public void EnemyKilled(int enemyID) {
        Level _level = levels[currentLevel];

        _level.waves[_level.getCurrentWave()].enemiesRemaining--;

        if (_level.waves[_level.getCurrentWave()].enemiesRemaining <= 0) {
            Debug.Log("Wave " + _level.getCurrentWave() + " complete!");
            _level.IncrementWave();
            if (_level.getCurrentWave() < _level.waves.Length) {
                SpawnWave();
            } else {
                LevelComplete();
            }
        }
    }

    private void SpawnWave() {
        Level _level = levels[currentLevel];

        if (_level.waves.Length <= 0) {
            LevelComplete();
            return;
        }

        _level.waves[_level.getCurrentWave()].SetEnemiesRemaining(_level.waves[_level.getCurrentWave()].enemies.Length);
        for (int i = 0; i < _level.waves[_level.getCurrentWave()].enemies.Length; i++) {
            Instantiate(_level.waves[_level.getCurrentWave()].enemies[i], _level.waves[_level.getCurrentWave()].spawnPoints[i].position, _level.waves[_level.getCurrentWave()].spawnPoints[i].rotation, enemiesParent.transform);
        }
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
}
