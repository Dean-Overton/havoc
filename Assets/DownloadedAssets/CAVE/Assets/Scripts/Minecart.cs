using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Minecart : MonoBehaviour
{
    // declaring the array of checkpoints ( * You will need to expand the "Size" of them based on how many Rails you have for this Minecart's route, and assign them in the fields, in ascending order * )
    public Transform[] checkpoints;
    // declaring the variable where the Minecart should move towards
    public Transform nextCheckpoint;
    // declaring the value for the speed at which the Minecard should move
    public float speed;
    // declaring the "Player" gameobject's Transform and Rigidbody
    public Transform player;
    public Rigidbody playerRb;
    // declaring the Transform variable, where the "Player" should be while aboard the Minecart
    public Transform Seat;
    // declaring the Transforms for the Front & Rear exit points
    public Transform FrontExit;
    public Transform RearExit;
    // declaring the minecart Audio (should be a component of this gameobject)
    public AudioSource minecartAud;
    // declaring a bool condition for "aboard" and "not aboard" (!aboard)
    public bool aboard;
    // declaring a bool conditions of the Minecart's direction
    public bool forward;
    // declaring an integer, which stores the value for the next checkpoint
    public int i = 0;
    //declaring the UI Text, which will display the input your player needs to press for the action to happen; YOU need to assign a UI text to this variable from your scene
    public Text inputText;
    //declaring a text field for the input, where you need to enter your own text
    public string myInputText;
    //declaring the UI Text, which will display the action which is going to happen if your player presses the input; YOU need to assign a UI text to this variable from your scene
    public Text actionText;
    //declaring a text field for the action, where you need to enter your own text
    public string myActionText;

    // Use this for initialization
    void Start()
    {
        // Fetching the "Player" tagged gameobject's Transform and Rigidbody
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        // Fetching the AudioSource, which is attached to this gameobject; and disabling it at the start of your game
        minecartAud = gameObject.GetComponent<AudioSource>();
        minecartAud.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Getting the "i" integer to match the "checkpoints" array current number (basically if i=4, nextCheckpoint=4, which is the 5th from the "checkpoints" array. *remember arrays start counting from 0)
        System.Array.IndexOf(checkpoints, i, 0, checkpoints.Length);

        // if the Player got "aboard":
        if (aboard)
        {
            // set the player's position to the "Seat"s position
            player.position = Seat.transform.position;
            // don't let the player's Rigidbody use gravity; set it to "isKinematic" (is getting moved around like a puppet) 
            playerRb.useGravity = false;
            playerRb.isKinematic = true;

            // if the Player clicks Left Mouse Button:
            if (Input.GetMouseButtonDown(0))
            {
                // change the "aboard" bool condition
                aboard = !aboard;
                // set the player's position to the "FrontExit"s position
                player.position = FrontExit.position;
            }

            // if the Player presses W Keycode:
            if (Input.GetKey(KeyCode.W))
            {
                // set forward condition to true
                forward = true;
                // the next checkpoint should be the "i"th from the "checkpoints" array
                nextCheckpoint = checkpoints[i];
                // make this gameobject "LookAt"/face the next checkpoint's position (because you want your minecart to always look forward / towards the next checkpoint)
                gameObject.transform.LookAt(nextCheckpoint.transform.position);
                // move this gameobject towards the next checkpoint's position, multiplied by the "speed" value
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextCheckpoint.position, speed * Time.deltaTime);
                // enable the Audio (it should be set to "Play on awake")
                minecartAud.enabled = true;
            }

            // if the "i" integer is the same or larger than the "checkpoints" array's length (meaning your minecart got to the last checkpoint):
            if (i >= checkpoints.Length)
            {
                // turn off the "aboard" condition
                aboard = false;
                // subtract -2 from the total amount of checkpoints, and make it the value for "i"
                i = (checkpoints.Length - 2);
                // set the player's position to the "FrontExit"s position
                player.position = FrontExit.position;
                // let the player's rigidbody use gravity; turn it off from "isKinematic"
                playerRb.useGravity = true;
                playerRb.isKinematic = false;
                // disable / stop the minecart Audio
                minecartAud.enabled = false;
            }

            // if the Player presses S Keycode:
            if (Input.GetKey(KeyCode.S))
            {
                // set forward condition to false (meaning it's traveling backwards now)
                forward = false;
                // the next checkpoint should be the "i"th from the "checkpoints" array
                nextCheckpoint = checkpoints[i];
                // make this gameobject "LookAt"/face the next checkpoint's position (because you want your minecart to always look forward / towards the next checkpoint)
                gameObject.transform.LookAt(nextCheckpoint.transform.position);
                // move this gameobject towards the next checkpoint's position, multiplied by the "speed" value
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextCheckpoint.position, speed * Time.deltaTime);
                // enable the Audio (it should be set to "Play on awake")
                minecartAud.enabled = true;
            }

            // if the "i" integer is the same or smaller than "-1" (meaning your minecart got back to the first checkpoint):
            if (i <= -1)
            {
                // turn off the "aboard" condition
                aboard = false;
                // set the "i" value to 1 (meaning the next checkpoint is now the 2nd from the array)
                i = 1;
                // set the player's position to the "RearExit"s position
                player.position = RearExit.position;
                // let the player's rigidbody use gravity; turn it off from "isKinematic"
                playerRb.useGravity = true;
                playerRb.isKinematic = false;
                // disable / stop the minecart Audio
                minecartAud.enabled = false;
            }

            // if the Player stops pressing the S Keycode:
            if (Input.GetKeyUp(KeyCode.S))
            {
                // disable / stop the minecart Audio
                minecartAud.enabled = false;
            }
            // if the Player stops pressing the W Keycode:
            if (Input.GetKeyUp(KeyCode.W))
            {
                // disable / stop the minecart Audio
                minecartAud.enabled = false;
            }
        }
        // or else, if the player is not aboard / "!aboard"
        else if (!aboard)
        {
            // let the player's rigidbody use gravity; turn it off from "isKinematic"
            playerRb.useGravity = true;
            playerRb.isKinematic = false;
        }
    }

    // if some gameobject entered the trigger zone...
    void OnTriggerStay(Collider col)
    {
        //... and the object is actually tagged "Player"
        if (col.CompareTag("Player"))
        {          
            if (!aboard)
            {
                //enabling the UI texts
                inputText.enabled = true;
                actionText.enabled = true;
                //setting the input & action UI texts'text to the one you entered in the "my...Text" fields
                inputText.text = myInputText;
                actionText.text = myActionText;
            }
            else if (aboard)
            {
                //disabling the UI texts, because player pressed the input & no longer needs to see the instruction texts
                inputText.enabled = false;
                actionText.enabled = false;
            }
            // if the Player releases the click of Left Mouse Button:
            if (Input.GetMouseButtonUp(0))
            {
                // change the "aboard" bool condition
                aboard = !aboard;
            }
        }
    }
    //the Player gameobject left the trigger zone
    void OnTriggerExit(Collider col)
    {
        //checking of the gameobject which left the trigger zone is actually tagged as "Player"
        if (col.CompareTag("Player") && !aboard)
        {
            //disabling the UI texts
            inputText.enabled = false;
            actionText.enabled = false;
            //set bool "opened" to false
        }
    }
}
