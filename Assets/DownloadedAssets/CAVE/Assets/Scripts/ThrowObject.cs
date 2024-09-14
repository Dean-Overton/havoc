using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour {

    // declaring the player's Transform, which you will need to calculate distance between throwable object and player ( * you need to drag your player gameObject into this field * )
    public Transform player;
    // declaring the camera's Transform, which you will need to parant the trowable object to
    public Transform cam;
    // declaring the Audio & the clips required for sound effects
    public AudioSource plAud;
    public AudioClip[] plClips;
    // declaring the value of the throw force ( * default is 0, and you will have to type a number into this field * )
    public float throwForce = 0f;
    // declaring the bool conditions for player being in range, or not
    bool hasPlayer = false;
    // declaring the bool conditions for the object being carried, or not
    bool beingCarried = false;

    // Use this for initialization
    void Start()
    {
        // fetching the main camera's Transform
        cam = Camera.main.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update () {
        // calculating the distance value, which is used as a range from the throwable object to the player
        float distance = Vector3.Distance(gameObject.transform.position, player.position);

        // if the player entered this range ( * the "1.5f" value can be changed to any value you want. I set 1.5 because it seemed like an average distance from which the player can pick up objects * )
        if(distance <= 1.5f)
        {
            // activate the condition of player being in range
            hasPlayer = true;
        }
        else // otherwise, if the player is not in range:
        {
            // deactivate the condition of player being in range
            hasPlayer = false;
        }

        // the player is in range, and *HOLDS DOWN*("GetMouseButtonDown") the Left MouseButton
        if (hasPlayer && Input.GetMouseButtonDown(0))
        {
            // set the throwable object's Rigidbody component to "isKinematic", which means it can be moved around
            GetComponent<Rigidbody>().isKinematic = true;
            // parenting this gameObject under the main camera, which means wherever the camera moves, this object moves along with it
            transform.parent = cam;
            // activating the condition for being carried
            beingCarried = true;
            // play the first sound from the "plClips" array once
            plAud.PlayOneShot(plClips[0]);
        }

        // the condition of being carried is active...
        if (beingCarried)
        {
            // ...and the player *RELEASES* ("GetMouseButtonUp") the Left MouseButton
            if (Input.GetMouseButtonUp(0))
            {
                // removing the throwable object's Rigidbody component from "isKinematic" condition, which means it can now be affected by gravity and other physics forces again
                GetComponent<Rigidbody>().isKinematic = false;
                // removing it from under the camera, which means it does not follow the camera around anymore
                transform.parent = null;
                // turning off the being carried condition
                beingCarried = false;
                // play the second sound from the "plClips" array once
                plAud.PlayOneShot(plClips[1]);
                // add a physics force to the released/thrown object, in the direction of the camera's forward (aka away from the camera), multiplied with the "throwForce" value
                GetComponent<Rigidbody>().AddForce(cam.forward * throwForce);
            }
        }
	}
}
