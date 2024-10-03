using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameController : MonoBehaviour {
    Scene currentScene;

    LevelController levelController;

    // This is for when this game object is first loaded in the main menu
    void Awake() {
        DontDestroyOnLoad(this.gameObject);     // Preventing the game object from being destroyed when a new scene loads
        levelController = gameObject.GetComponent<LevelController>();

        EventManager.OnCharacterDeath += CharacterDied;
        EventManager.OnNewLevelEnter += NewLevelEntered;

        currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex == 1){
            OnNewGame();
        }
    }

    void OnNewGame() {
        levelController.BeginGame();
    }

    void NewLevelEntered(int levelID) {
        Debug.Log("New level entered: " + levelID);
        levelController.StartNewLevel(levelID);
    }

    void CharacterDied(int characterID) {
        if (characterID == 1) {
            Debug.Log("Player has died!");
            // Reload the scene
            SceneManager.LoadScene(1);
        } else if (characterID >= 2) {
            // Debug.Log("Enemy has died!");
            // Level controller wave[currentWave].enemiesRemaining--;
           levelController.EnemyKilled(characterID);
        }
    }

    void OnDestroy() {
        // Unsubscribe from the event to prevent memory leaks
        EventManager.OnCharacterDeath -= CharacterDied;
        EventManager.OnNewLevelEnter -= NewLevelEntered;
    }

}