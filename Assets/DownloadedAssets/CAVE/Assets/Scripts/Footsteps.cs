using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour {

    // declaring the AudioSource and the Clips required for this function
    public AudioSource footstepsAud;
    public AudioClip[] footstepsClips;
    // declaring the player's Rigidbody
    public Rigidbody playerRb;
    // a float variable for checking the player's Ridigbody's velocity
    public float speed;

    // Use this for initialization
    void Start () {
        // fetching the AudioSource, which is attached to this gameObject
        footstepsAud = gameObject.GetComponent<AudioSource>();
        // setting the first [0] AudioClip from the "footstepsClips" array as default at the start of your game
        footstepsAud.clip = footstepsClips[0];
    }
	
	// Update is called once per frame
	void Update () {
        // fetch the velocity of the player's Rigidbody as your "speed" variable
        speed = playerRb.velocity.magnitude;
        // check if there is speed...
        if (speed < 0.1f)      
        {
            // ...then play the Sound 
            footstepsAud.Play();
        }
    }

    // the Feet gameObject is colliding with something...
    private void OnTriggerEnter(Collider col)
    {
        // ...which is tagged as "Wood"
        if (col.CompareTag("Wood"))
        {
            // change the AudioClip (to be played) into the second from the "footstepsClips" array 
            footstepsAud.clip = footstepsClips[1];
            // play the Sound
            footstepsAud.Play();
        }
    }

    // the Feet gameObject isn't colliding anymore, with something...
    private void OnTriggerExit(Collider col)
    {
        // ...which is tagged as "Wood"
        if (col.CompareTag("Wood"))
        {
            // change the AudioClip (to be played) back into the first from the "footstepsClips" array 
            footstepsAud.clip = footstepsClips[0];
            // play the Sound
            footstepsAud.Play();
        }

        // * * * Example for additional sound * * *   erase the "//" at the beginning of below lines

        //if (col.CompareTag("YourCustomTag"))
        //{
            //footstepsAud.clip = footstepsClips[3];   //assuming it's the THIRD sound you setup
            //footstepsAud.Play();
        //}
    }
}
