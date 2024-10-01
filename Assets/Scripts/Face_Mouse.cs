using UnityEngine;

public class CharacterFacing : MonoBehaviour
{
    [SerializeField]
    private bool RotateTowardMouse = true; // Whether the character should rotate towards the mouse position

    [SerializeField]
    private float RotationSpeed = 500f; // Speed at which the character rotates

    [SerializeField]
    private Camera Camera; // Reference to the camera used to calculate mouse position

    void Update()
    {
        if (RotateTowardMouse)
        {
            // Rotate the player based on mouse position
            RotateFromMouseVector();
        }
    }

    private void RotateFromMouseVector()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        // Calculate the Y-level where the player is
        float playerY = transform.position.y;

        // Calculate how far we need to move along the ray to reach the player's Y-coordinate
        if (ray.direction.y != 0)
        {
            float distance = (playerY - ray.origin.y) / ray.direction.y;

            // Calculate the target point at the player's Y-level
            Vector3 targetPoint = ray.origin + ray.direction * distance;

            // Rotate the character to look at the target point (only on XZ plane)
            Vector3 directionToTarget = targetPoint - transform.position;
            directionToTarget.y = 0; // Ensure rotation is only on the XZ plane

            // Calculate the rotation and apply it smoothly
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }
}