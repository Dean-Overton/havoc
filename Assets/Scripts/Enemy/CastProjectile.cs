using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CastProjectile : MonoBehaviour
{
    public Vector3 targetPosition;

    public Vector3 startDirection;

    void Start() {
        // Set the target position to the player's position
        targetPosition = GameObject.Find("Player").transform.position;
        // Set the start direction to the forward direction of the enemy
        startDirection = targetPosition - transform.position;
    }
    public float verticalVolocityNeeded = 5f;
    private void Update() {
        if(CheckPossiblePath()) {
            verticalVolocityNeeded = CalculateForceRequired();
        }
    }
    public void Fire() {
        // Add force to the rigidbody
        // Add force in the direction of the target position

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(startDirection * verticalVolocityNeeded, ForceMode.Impulse);
    }
    bool CheckPossiblePath() {
        // Check x and z normalised direction is the sama as difference between target and start position
        Vector3 newStartNormalised = new Vector3(startDirection.x, 0, startDirection.z);
        Vector3 normalisedDirection = newStartNormalised.normalized;
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();
        return normalisedDirection == targetDirection;

        // Check if vertical flight time 
    }
    float CalculateForceRequired () {
        return 10f;
    }
}
