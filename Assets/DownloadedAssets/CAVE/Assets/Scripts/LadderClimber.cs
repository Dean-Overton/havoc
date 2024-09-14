using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LadderClimber : MonoBehaviour {

    // declaring the Transform and Rigidbody of the player gameObject
	public Transform player;
    public Rigidbody playerRb;
    // declaring the Transform of this object, and the AudioSource
	private Transform ladderTransform;
    public AudioSource ladderAUD;
    // declaring a bool for the condition of climbing and not climbing
    public bool Climbing;
    // the speed at which the player moves along the ladder
    public float speed = 1f;
    //declaring the UI Text, which will display the input your player needs to press for the action to happen; YOU need to assign a UI text to this variable from your scene
    public Text inputText;
    //declaring a text field for the input, where you need to enter your own text
    public string myInputText;
    //declaring the UI Text, which will display the action which is going to happen if your player presses the input; YOU need to assign a UI text to this variable from your scene
    public Text actionText;
    //declaring a text field for the action, where you need to enter your own text
    public string myActionText;

    // Use thie for initialization
    void Start () {
        // fetching the Transform and Rigidbody of your player gameObject, which must be tagged "Player"
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        // declaring that the "ladderTransform" is this gameObject's transform
        ladderTransform = transform;
        // fetching the AudioSource, which is attached to this gameObject
        ladderAUD = gameObject.GetComponent<AudioSource>();
        // disabling the Audio at the start of your game
        ladderAUD.enabled = false;
    }
	
    // if something enter the trigger zone...
	void OnTriggerStay(Collider other)
	{
        // ...and it's tagged as "Player"
        if (other.CompareTag("Player")){
            if (!Climbing)
            {
                //enabling the UI texts
                inputText.enabled = true;
                actionText.enabled = true;
                //setting the input & action UI texts'text to the one you entered in the "my...Text" fields
                inputText.text = myInputText;
                actionText.text = myActionText;
            }
            else if (Climbing)
            {
                //disabling the UI texts, because player pressed the input & no longer needs to see the instruction texts
                inputText.enabled = false;
                actionText.enabled = false;
            }
            // the player Clicks Left MouseButton & "Climbing" condition is false
            if (Input.GetMouseButtonDown(0) && !Climbing) {
                // set the player's position to match the position of the ladder
                player.position = new Vector3 (player.position.x, player.position.y, ladderTransform.position.z);
                // enabling the "Climbing" condition
                Climbing = true;
				}
		}
        // when "Climbing" is enabled:
		if (Climbing) {
            // turning off grafity for the player's Rigidbody
            playerRb.useGravity = false;
            // set the player's position to match the position of the ladder AGAIN TO BE SURE
            player.position = new Vector3 (player.position.x, player.position.y, ladderTransform.position.z);
                // the player presses "W" Key on his keyboard:
                if (Input.GetKey(KeyCode.W)){
                    // translate/move the player upwards with the speed you set at the "speed" field, while playing the Audio
                    player.Translate(Vector3.up * Time.deltaTime*speed);
                    ladderAUD.enabled = true;
			    }
                // the player presses "S" Key on his keyboard: 
                else if(Input.GetKey(KeyCode.S)){
                    // translate/move the player downwards with the speed you set at the "speed" field, while playing the Audio
                    player.Translate(Vector3.down * Time.deltaTime*speed, Space.World);
                    ladderAUD.enabled = true;
                }
		}
	}

    // if something exited/left the trigger zone...
	void OnTriggerExit(Collider other)
	{
        // ...and it's tagged as "Player"
        if (other.CompareTag("Player"))
        {
            //disabling the UI texts
            inputText.enabled = false;
            actionText.enabled = false;
            // turning back on grafity for the player's Rigidbody
            playerRb.useGravity = true;
            // translate/move the player forward a bit
            player.Translate(Vector3.forward * Time.deltaTime * 20);
            // disable the "Climbing" condition
            Climbing = false;
            // disable the Audio from being played
            ladderAUD.enabled = false;
        }
	}
}
