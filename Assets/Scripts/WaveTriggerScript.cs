using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerScript : MonoBehaviour
{
    [SerializeField]
    private int levelToTrigger = 0;
    private GameObject _player;
    void Awake()
    {
        _player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            EventManager.BroadcastNewLevelEnter(levelToTrigger);
            Destroy(gameObject);
        }
    }
}
