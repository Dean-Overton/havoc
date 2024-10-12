using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameController : MonoBehaviour {
    public static GameController instance;
    Scene currentScene;

    LevelController levelController;

    // This is for when this game object is first loaded in the main menu
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);     // Preventing the game object from being destroyed when a new scene loads
            levelController = gameObject.GetComponent<LevelController>();

            EventManager.OnCharacterDeath += CharacterDied;
            EventManager.OnNewLevelEnter += NewLevelEntered;

            // currentScene = SceneManager.GetActiveScene();
            // if (currentScene.buildIndex == 1){
            //     Invoke("OnNewGame", 1f);
            // }
            SceneManager.sceneLoaded += OnSceneLoaded;
        } else {
            instance = gameObject.GetComponent<GameController>();
            Destroy(this.gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 1) {
            Debug.Log("Scene loaded --- 1: " + scene.name);
            Invoke("OnNewGame", 1f);
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
        if (characterID == 0) {
            return;
        }
        else if (characterID == 1) {
            Debug.Log("Player has died!");
            StartCoroutine(OnPlayerDeath());
        } else if (characterID >= 2) {
           levelController.EnemyKilled(characterID);
        }
        // Add more logic for different enemies
    }

    private IEnumerator OnPlayerDeath() {

        yield return new WaitForSeconds(2f);

        // Reload the scene
        SceneManager.LoadScene(1);
        Destroy(this.gameObject);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    void OnDestroy() {
        // Unsubscribe from the event to prevent memory leaks
        EventManager.OnCharacterDeath -= CharacterDied;
        EventManager.OnNewLevelEnter -= NewLevelEntered;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}