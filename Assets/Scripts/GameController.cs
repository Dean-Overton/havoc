using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    Scene currentScene;

    health_component playerHealthScript;

    GameObject playerBody;

    // This is for when this game object is first loaded in the main menu
    void Awake() {
        DontDestroyOnLoad(this.gameObject);     // Preventing the game object from being destroyed when a new scene loads

        currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex > 0){
            OnNewLevelLoad();
        }
    }

    void OnNewLevelLoad() {
        Debug.Log(currentScene.buildIndex);
    
        playerBody = GameObject.FindGameObjectWithTag("PlayerBody");

        playerHealthScript = playerBody.GetComponent<health_component>();

        Debug.Log(playerHealthScript.getMaxHealth());

        playerHealthScript.OnPlayerDeath += PlayerDied;
    }

    void PlayerDied() {
        Debug.Log("PLAYER HAS DIED!!!!!!!!!");
    }

}
