using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    UIDocument mainMenuDocument;

    Button newGameButton;
    void OnEnable() {
        mainMenuDocument = GetComponent<UIDocument>();

        newGameButton = mainMenuDocument.rootVisualElement.Q("NewGame-Button") as Button;

        newGameButton.RegisterCallback<ClickEvent>(OnNewGameClick);
    }

    public void OnNewGameClick(ClickEvent e) {
        Debug.Log("NEW GAME!!!");
        // Remove main menue

        // Enable Normal UI
    }
    void Start() {
        // Play some music

        // On (New Game or Continue) button pressed
            // stop playing music        
    }

}
// https://www.youtube.com/watch?v=_jtj73lu2Ko