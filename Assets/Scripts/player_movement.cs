using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    [SerializeField]
    private float MovementSpeed; // Speed at which the character moves

    [SerializeField]
    private Camera Camera; // Reference to the camera used to calculate mouse position

    private Vector2 _inputVector; // Stores player input

    void Update()
    {
        // Collect input from the player
        GetInput();

        // Create a target movement vector based on input
        var targetVector = new Vector3(_inputVector.x, 0, _inputVector.y);

        // Move the character towards the target direction
        MoveTowardTarget(targetVector);
    }

    private void GetInput()
    {
        // Get horizontal and vertical input from the player (usually WSAD or arrow keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a vector based on the input
        _inputVector = new Vector2(moveX, moveZ);
    }

    private void MoveTowardTarget(Vector3 targetVector)
    {
        // Calculate the movement speed adjusted for frame time
        var speed = MovementSpeed * Time.deltaTime;

        // Adjust the target vector based on the camera’s rotation
        targetVector = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0) * targetVector;

        // Calculate the new position based on the movement vector
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
    }
}
