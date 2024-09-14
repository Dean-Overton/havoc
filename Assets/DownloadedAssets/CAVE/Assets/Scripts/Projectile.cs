using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // declaring the variable (seconds) in which the Projectile will disappear
    public float expiryTime = 0f;

    // Use this for initialization
	void Start () {
        // the gameObject will self destruct in "expirtyTime" seconds...
        Destroy(gameObject, expiryTime);
	}
}
