using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _document;
    private Button newGameButton;
    private Scene currentScene;

    private void Start() {
        if (currentScene.buildIndex == 0) {
            _document = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UIDocument>();  // Getting the UI document from the main camera game object
            newGameButton = _document.rootVisualElement.Q("NewGame-Button") as Button;      // Getting the "New Game" button from the document
            newGameButton.RegisterCallback<ClickEvent>(OnNewGame);      // Making this button a click event listener
            
            AudioManager.instance.Play("MainMenuMusic");      // Play the main menu music
        }
    }

    // De-registering the callback function as good practice to avoid a possible bug of this being called outside the main menu
    private void OnDisable() {
        if (currentScene.buildIndex == 0) {
            newGameButton.UnregisterCallback<ClickEvent>(OnNewGame);
        }
    }

    // When the New Game button is clicked, or any other instance where a new game is requested
    void OnNewGame(ClickEvent click) {
        AudioManager.instance.StopPlaying("MainMenuMusic");      // Stop the main menu music

        SceneManager.LoadSceneAsync(1);
    }

}
