using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    [SerializeField]
    private float MovementSpeed = 5f;      // Speed at which the character moves
    [SerializeField]
    private Camera Camera;                 // Reference to the camera used to calculate mouse position
    [SerializeField]
    private float gravity = -9.81f;        // Gravity force
    [SerializeField]
    private float groundCheckDistance = 0.1f;  // Distance to check for the ground

    private Vector2 _inputVector;          // Stores player input
    private CharacterController _characterController;  // Reference to the CharacterController component
    private Vector3 _velocity;             // Velocity to apply gravity
    private bool _isGrounded;              // Whether the player is on the ground

    private Animator animator;

    void Start()
    {
        // Get the CharacterController component on the player object
        _characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if the player is grounded by casting down from the character
        GroundCheck();

        // Collect input from the player
        GetInput();

        // Create a target movement vector based on input
        var targetVector = new Vector3(_inputVector.x, 0, _inputVector.y);

        // Move the character towards the target direction using CharacterController
        MoveTowardTarget(targetVector);

        // Apply gravity
        ApplyGravity();
    }

    private void GroundCheck()
    {
        // Check if the character is on the ground
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + _characterController.height / 2);

        // Reset velocity on the Y-axis if grounded
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;  // Small value to keep the character grounded
        }
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

        // Move the character using the CharacterController component (with horizontal movement)
        _characterController.Move(targetVector * speed);

        // Calculate the magnitude of the targetVector (this represents the movement speed)
        float movementMagnitude = targetVector.magnitude;

        // Set the speed in the animator based on the magnitude of the movement
        animator.SetFloat("Speed", movementMagnitude);
        Debug.Log(animator.GetFloat("Speed"));
    }

    private void ApplyGravity()
    {
        // Apply gravity when not grounded
        if (!_isGrounded)
        {
            _velocity.y += gravity * Time.deltaTime; // Increase downward velocity over time
        }

        // Apply the Y velocity (gravity) to the character
        _characterController.Move(_velocity * Time.deltaTime);
    }
}