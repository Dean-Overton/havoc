using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(this.gameObject);     // Preventing the game object from being destroyed when a new scene loads
    }
}
