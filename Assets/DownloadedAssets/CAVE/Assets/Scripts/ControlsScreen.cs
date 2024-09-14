using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsScreen : MonoBehaviour
{
    // the canvas where your Controls Image is displayed
    public GameObject controlsScreen;

    // Use this for initialization
    void Start()
    {
        // at the start of your game, the Controls Image should be active for display; the timeScale should be 0 (time is frozen in place)
        controlsScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // if player Clicks Left MouseButton
        if (Input.GetMouseButtonDown(0))
        {
            // the Controls Image is deactivated
            controlsScreen.SetActive(false);
            // time returns to 1 (normal)
            Time.timeScale = 1f;
            // this gameObject will self destruct in 1 second...
            Destroy(gameObject, 1);
        }
    }

}
