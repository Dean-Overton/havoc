using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// assuming you have Unity's PostProcessing asset incorporated into Your Project
namespace UnityEngine.PostProcessing
{
    public class PostFXenabler : MonoBehaviour
    {
        // declaring the GameObject which is running the PostFX
        public GameObject postFX;
        // declaring a bool to store MenuOn / MenuOff conditions
        public bool settingsMenu;
        // declaring the actual menu required for this function
        public GameObject menu;

        // Use this for initialization
        void Start()
        {
            // turning the PostFX on
            postFX.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            // if player presses Esc Button
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // switch the conditions for MenuOn / MenuOff
                settingsMenu = !settingsMenu;
            }

            // checking if Menu should be turned on
            if (settingsMenu)
            {
                // activate the "menu"; make the Cursor visible
                menu.SetActive(true);
                Cursor.visible = true;
            }
            // checking if Menu should be turned off
            else if (!settingsMenu)
            {
                // inactivate the "menu"; make the Cursor invisible
                menu.SetActive(false);
                Cursor.visible = false;
            }
        }

        // * * * This Function needs to be called on the "Button-enable" UI Button * * *
        public void EnableFX()
        {
            // enable the PostFX GameObject to do it's magic; switch conditions again
            postFX.SetActive(true);
            settingsMenu = !settingsMenu;
        }

        // * * * This Function needs to be called on the "Button-disable" UI Button * * *
        public void DisableFX()
        {
            // disable the PostFX GameObject :( and switch conditions again
            postFX.SetActive(false);
            settingsMenu = !settingsMenu;
        }
    }
}
