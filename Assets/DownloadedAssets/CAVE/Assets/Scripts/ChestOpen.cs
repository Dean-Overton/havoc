using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestOpen : MonoBehaviour {

    public Animator chestAnim;
    public AudioSource chestAud;
    public AudioClip[] chestClips;
    //declaring the UI Text, which will display the input your player needs to press for the action to happen; YOU need to assign a UI text to this variable from your scene
    public Text inputText;
    //declaring a text field for the input, where you need to enter your own text
    public string myInputText;
    //declaring the UI Text, which will display the action which is going to happen if your player presses the input; YOU need to assign a UI text to this variable from your scene
    public Text actionText;
    //declaring a text field for the action, where you need to enter your own text
    public string myActionText;
    public bool opened;
    public GameObject item = null;

	// Use this for initialization
	void Start () {

        // fetching the Animator and AudioSource, which are components attached to this gameObject
        chestAnim = gameObject.GetComponent<Animator>();
        chestAud = gameObject.GetComponent<AudioSource>();
        // setting "opened" to false (a.k.a. closed) at start of your game
        opened = false;
	}
	
	// Update is called once per frame
	void Update () {

        //  *  *  *  *  *  *  *  ONLY USE THIS PART if you have a Child GameObject attached, and you want this script to Activate it when the Chest is opened:
        #region
            //checking if player opened the Chest, and there is an item assigned, which should now appear
            if (opened && item != null)
            {
                //activating the item GameObject, so the player can take it
                item.SetActive(true);
            }
            //checking if the Chest closed, if there is an item assigned...
            else if (!opened && item != null)
            {
            //...then that item should now disappear/ inactivate
                item.SetActive(false);
            }
        #endregion //  *  *  *  *  *  *  *  if you don't want an item activated when the Chest opens, CUT OUT THIS REGION

    }

    //the Player gameobject entered the trigger zone (is in range of opening chest)
    void OnTriggerStay(Collider col)
    {
        //checking of the gameobject is actually tagged as "Player"
        if (col.CompareTag("Player"))
        {
            //enabling the UI texts
            if (!opened)
            {
                inputText.enabled = true;
                actionText.enabled = true;
                //setting the input & action UI texts'text to the one you entered in the "my...Text" fields
                inputText.text = myInputText;
                actionText.text = myActionText;
            }
            else if (opened)
            {
                //disabling the UI texts, because player pressed the input & no longer needs to see the instruction texts
                inputText.enabled = false;
                actionText.enabled = false;
            }
            //if player Clicks Left MouseButton & the chest is still closed
            if (Input.GetMouseButtonUp(0) && !opened)
            {
                //set bool "opened" to true; play "Chest-open" animation; play first (open) sound
                opened = true;
                chestAnim.SetBool("open", true);
                chestAud.clip = chestClips[0];
                chestAud.Play();
            }
        }
    }

    //the Player gameobject left the trigger zone (is out of range, so chest can close back now)
    void OnTriggerExit(Collider col)
    {
        //checking of the gameobject which left the trigger zone is actually tagged as "Player"
        if (col.CompareTag("Player") && opened)
        {
            //disabling the UI texts
            inputText.enabled = false;
            actionText.enabled = false;
            //set bool "opened" to false
            opened = false;
            //play "Chest-close" animation
            chestAnim.SetBool("open", false);
            //set the clip to the audioSource & play second(close) sound
            chestAud.clip = chestClips[1];
            chestAud.Play();
        }
        if (col.CompareTag("Player") && !opened)
        {
            //disabling the UI texts
            inputText.enabled = false;
            actionText.enabled = false;
        }
    }
}
