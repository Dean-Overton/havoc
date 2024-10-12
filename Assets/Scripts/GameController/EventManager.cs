using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CharacterDied(int characterID);
    public static event CharacterDied OnCharacterDeath;
    public static void BroadcastCharacterDeath(int characterID) {
        OnCharacterDeath?.Invoke(characterID);
    }

    public delegate void NewLevelEnter(int levelID);
    public static event NewLevelEnter OnNewLevelEnter;
    public static void BroadcastNewLevelEnter(int levelID) {
        OnNewLevelEnter?.Invoke(levelID);
    }

    public delegate void GameComplete();
    public static event GameComplete OnGameComplete;
    public static void BroadcastGameComplete() {
        OnGameComplete?.Invoke();
    }
}