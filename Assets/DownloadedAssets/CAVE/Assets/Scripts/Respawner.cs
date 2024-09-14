using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    // declaring the player gameobject (must be tagged with "Player")
    public GameObject player;
    // declaring the spawn point where the player will be reset ( * in the Prefabs folder there is a "Reseter" Prefab, which already has a spawn child object under it * )
    public Transform spawn;
    // declaring the Audio which will be player upon reset (must be a component of this gameObject)
    private AudioSource drums;

    // Use this for initialization
	void Start () {

        // fetching the player gameObject, which is tagged "Player" and the AudioSource component attached to this gameObject
        player = GameObject.FindGameObjectWithTag("Player");
        drums = gameObject.GetComponent<AudioSource>();
	}
	
    // if something entered the trigger zone...
    private void OnTriggerEnter(Collider col)
    {
        // ...and is tagged "Player"
        if (col.CompareTag("Player"))
        {
            // set the player's position to the position of the gameObject you assigned in the "spawn" field ( * in the Prefabs folder there is a "Reseter" Prefab, which already has a spawn setup * )
            player.transform.position = spawn.position;
            // play the Audio
            drums.Play();
        }
    }
}
