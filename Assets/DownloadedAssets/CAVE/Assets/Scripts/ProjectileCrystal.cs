using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCrystal : MonoBehaviour {

    // declaring the variable (seconds) in which the Projectile will disappear
    public float expiryTime = 0f;
    // declaring the shattered crystal particle GameObject
    public GameObject particleDestroyed;
    // declaring the Mesh GameObject
    public GameObject crystal;
    // declaring the Audio which will play the breaking crystal sound
    public AudioSource aud;
    // the bool which stores the condition for the crystal's readyness to break ( * once it has broken it should not break again and again * )
    private bool canBreak;

    // Use this for initialization
    void Start()
    {
        // when the crystal is instanced/thrown, you want the shattered particle to be inactive
        particleDestroyed.SetActive(false);
        // fetching the AudioSource, which is a component of this gameObject
        aud = gameObject.GetComponent<AudioSource>();
        // when the crystal is instanced/thrown, you want it to be able to break once it hits something
        canBreak = true;
        // this gameObject will self desctruct in "expirtyTime" seconds...
        Destroy(gameObject, expiryTime);
    }

    // if the gameObject collides with something
    private void OnCollisionEnter(Collision collision)
    {
        // now the shattered particle should get ativated
        particleDestroyed.SetActive(true);
        // the Mesh (together with it's Light child) should disappear
        crystal.SetActive(false);

        // if up until now the condition was "canBreak", and now that it collided with something:
        if (canBreak)
        {
            // play the break effect Audio
            aud.Play();
            // cannot break any further
            canBreak = false;
        }
    }
}
