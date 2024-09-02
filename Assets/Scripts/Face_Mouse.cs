using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFacing : MonoBehaviour
{
    [SerializeField]
    private bool RotateTowardMouse = true; // Whether the character should rotate towards the mouse position

    [SerializeField]
    private float RotationSpeed = 500f; // Speed at which the character rotates

    [SerializeField]
    private float LockOnDistance = 10f; // Maximum distance for lock-on

    [SerializeField]
    private Camera Camera; // Reference to the camera used to calculate mouse position

    private GameObject _lockedOnObject; // The currently locked-on object

    void Update()
    {
        if (_lockedOnObject != null)
        {
            // Check if the mouse position is within lock-on distance of the locked-on object
            if (IsMouseWithinLockOnDistance())
            {
                // Rotate towards the locked-on object
                RotateTowardLockedOnObject();
            }
            else
            {
                // If out of range, stop locking on
                _lockedOnObject = null;
            }
        }
        else if (RotateTowardMouse)
        {
            // Rotate towards the mouse position
            RotateFromMouseVector();
            // Check if the mouse hits a valid lock-on target
            CheckForLockOn();
        }
    }

    private void RotateFromMouseVector()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        // Check if the ray hits any object
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Get the point where the ray hit the object
            Vector3 target = hitInfo.point;
            target.y = transform.position.y; // Keep the target on the same vertical level as the character
            Debug.Log("trying to look at " +  target);

            // Rotate the character to look at the target point
            Vector3 directionToTarget = target - transform.position;
            directionToTarget.y = 0; // Ensure rotation is only on the XZ plane
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private void RotateTowardLockedOnObject()
    {
        if (_lockedOnObject != null)
        {
            // Rotate towards the locked-on object
            Vector3 directionToLockedOnObject = _lockedOnObject.transform.position - transform.position;
            directionToLockedOnObject.y = 0; // Ensure rotation is only on the XZ plane
            Quaternion rotation = Quaternion.LookRotation(directionToLockedOnObject);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        }
    }

    private bool IsMouseWithinLockOnDistance()
    {
        if (_lockedOnObject != null)
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
           
            // Check if the ray hits any object
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Calculate the distance from the mouse position to the locked-on object
                float distanceToLockedOnObject = Vector3.Distance(hitInfo.point, _lockedOnObject.transform.position);

                // Check if this distance is within the lock-on range
                return distanceToLockedOnObject <= LockOnDistance;
            }
        }

        // If raycast does not hit anything or locked-on object is null, consider it out of range
        return false;
    }

    private void CheckForLockOn()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        // Check if the ray hits any object
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Check if the object has a "lock_on_able" tag or component
            if (hitInfo.collider.CompareTag("LockOnAble")) // Make sure to set the correct tag or add a component check
            {
                // Set the locked-on object
                _lockedOnObject = hitInfo.collider.gameObject;
            }
        }
    }
}
