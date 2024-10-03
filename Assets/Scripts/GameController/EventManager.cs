using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Define a delegate and a static event
    public delegate void CharacterDied(int characterID);
    public static event CharacterDied OnCharacterDeath;
    // Method to invoke the event
    public static void BroadcastCharacterDeath(int characterID)
    {
        OnCharacterDeath?.Invoke(characterID);
    }

    public delegate void NewLevelEnter(int levelID);
    public static event NewLevelEnter OnNewLevelEnter;
    public static void BroadcastNewLevelEnter(int levelID)
    {
        OnNewLevelEnter?.Invoke(levelID);
    }
}