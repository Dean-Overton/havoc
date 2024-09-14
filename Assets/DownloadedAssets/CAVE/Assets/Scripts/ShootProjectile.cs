using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // you require this line to declare UI stuff

public class ShootProjectile : MonoBehaviour {

    // declaring the GameObject, which will be your current Projectile ( * drag Projectile Prefab into this field * )
    public GameObject projectile = null;
    // the Transform/origin point, where the Projectile should be instanced/thrown from
    public Transform gun;
    // declaring the UI Image to display your "currentItem" ( * drag your UI Image from the scene into this field * )
    public Image currentItem;
    // declaring the UI Text to display your current ammo (numbers) ( * drag your UI Text from the scene into this field * )
    public Text currentAmmoText;
    // declaring an int for the number of ammo (int stores whole numbers)
    public int ammoCount;
    // the variable (seconds) between shots
    public float shootRate = 0f;
    // the variable for the magnitude/force of your shot (higher values will shoot projectiles further)
    public float shootForce = 0f;
    // the variable which calculates the shoot rate
    private float ShootRateTimeStamp = 0f;

	// Update is called once per frame
	void Update () {
        // setting the UI Text to match your "ammoCount"
        currentAmmoText.text = ammoCount.ToString(); 

        // the player presses Right MouseButton & "projectile" field does not equal to null (there actually is something assigned in that field) & the "ammoCount" is larger than 0 (player has ammo to shoot)
        if (Input.GetButtonDown("Fire2") && projectile != null && ammoCount > 0)
        {
            // the shoot rate is ready (enough seconds have passed to match the "shootRate")
            if (Time.time > ShootRateTimeStamp)
            {
                // instantiate/summon the "projectile" Prefab at the "gun"s position and rotation (origin point)
                GameObject bullet = (GameObject)Instantiate(
                    projectile, gun.position, gun.rotation);
                // add a physics force to this newly instantiated GameObject, multiplied with the "shootForce" value
                bullet.GetComponent<Rigidbody>().AddForce(gun.forward * shootForce);
                // start counting down the seconds needed for the next shot
                ShootRateTimeStamp = Time.time + shootRate;
                // subtract (-=) 1 ammo 
                ammoCount -= 1;
            }
        }
	}
}
