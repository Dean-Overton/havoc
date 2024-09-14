using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    // your mesh and audio assets required for this function
    public GameObject model;
    public GameObject sphereExplosion;
    public AudioSource explosionAud;
    // the power of the explosion physics force
    public float power = 0f;
    // the radius of gameObjects to be affected by physics force
    public float radius = 0f;
    // the physics up-force applied for dramatic explosion effect where the objects slightly lift
    public float liftForce = 0f;
    // Nr of Seconds until detonation
    public float countdown = 0f;

    // Use this for initialization
    void Start () {
        // fetching the AudioSource, which is attached to this gameObject
        explosionAud = gameObject.GetComponent<AudioSource>();
        // turning both the explosion effect Sphere and the explosion sound off when the Dynamite-Explosive gameObject was instanced/activated
        sphereExplosion.SetActive(false);
        explosionAud.enabled = false;
    }
	
	void FixedUpdate () {
        // start running the "Detonate" function below in your Nr of Seconds set at the "countdown" field
        Invoke("Detonate", countdown);
	}

    // the function which creates the explosion effect
    void Detonate()
    {
        // the mesh asset of the Dynamite will be destroyed instantly
        Destroy(model);
        // the entire Dynamite-Explosive gameObject will self destruct in 2 seconds...
        Destroy(gameObject, 2);
        // turn the sphere asset effect ON
        sphereExplosion.SetActive(true);
        // enable the BOOOOOOM sound
        explosionAud.enabled = true;

        // set the origin point for the explosion effect at the center of wherever the Dynamite-Explosive gameObject ended up in upon Detonation
        Vector3 explosionPosition = gameObject.transform.position;
        // fetch all of the Colliders, which are within the spheric range/radius
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);

        // what to do with all those Colliders, which got touched/hit inside the spheric range/radius
        foreach (Collider hit in colliders)
        {
            // fetch their Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            // if they DO HAVE (!= null) a Rigidbody
            if (rb != null)
            {
                // add the explosion physics force to them (combining the "power", "liftForce", and distance from the explosion center variables)
                rb.AddExplosionForce(power, explosionPosition, radius, liftForce, ForceMode.Impulse);
            }
        }
    }
}
