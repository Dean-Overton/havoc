using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    // declaring the Light and the AudioSource required for this function
    public Light flashlight;
    public AudioSource lightAud;
    // a bool condition for the Light to be on/off
    public bool lightOn;

    // Use this for initialization
    void Start () {
        // you would want the Light turned off at the game start, if the player hasn't activated it before
        flashlight.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        // if the player Presses the Mouse Scroll Button
        if (Input.GetButtonDown("Fire3"))
        {
            // change the on/off value 
            lightOn = !lightOn;
            // play the Audio
            lightAud.Play();
        }

        // in the condition of Light being On
        if (lightOn)
        {
            // turn on the light
            flashlight.enabled = true;
        }
        // or else, in the condition of Light being Off  *  *  * sometimes you should also have the opposite condition stated below in an "else if" *  *  *
        else if (!lightOn)
        {
            // turn off the light
            flashlight.enabled = false;
        }
    }
}
