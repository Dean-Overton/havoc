using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerScript : MonoBehaviour
{
    [SerializeField] private int levelToTrigger = 0;
    [SerializeField] private GameObject[] InitialEnemies;

    void Update() {
        bool allEnemiesDestroyed = true;

        foreach (GameObject enemy in InitialEnemies)
        {
            if (enemy != null)
            {
                allEnemiesDestroyed = false;
                break;
            }
        }

        if (allEnemiesDestroyed)
        {
            EventManager.BroadcastNewLevelEnter(levelToTrigger);
            Destroy(gameObject);
        }
    }
}