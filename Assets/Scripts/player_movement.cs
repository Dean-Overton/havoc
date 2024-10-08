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
    public bool isMovingBackwards;


    //TODO

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

        // Convert input vector to local space relative to the character's orientation
        Vector3 localInput = transform.InverseTransformDirection(targetVector);

        // Set the converted local space values to the animator
        animator.SetFloat("Horizontal", localInput.x);  // Set Horizontal (left/right strafing)
        animator.SetFloat("Vertical", localInput.z);    // Set Vertical (forward/backward movement)

        // You can check if the character is moving backwards or forwards based on localInput.z
        bool isMovingBackwards = localInput.z < 0;
        animator.SetBool("isRunningBackwards", isMovingBackwards);
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
        // Normalize the target vector to prevent faster diagonal movement
        if (targetVector.magnitude > 1f)
        {
            targetVector.Normalize();  // Ensures diagonal movement doesn't exceed max speed
        }

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
        bool isMovingBackwards = CheckIfMovingBackwards(targetVector);
        animator.SetBool("isRunningBackwards", isMovingBackwards);
        // Only rotate the character if there is movement, and the character is not shooting
        if(GetComponent<GunController>().isShooting == false)  {
            if (targetVector.magnitude > 0.1f)
            {
                // Determine the rotation direction
                Quaternion toRotation = Quaternion.LookRotation(targetVector);

                // Smoothly rotate the player character towards the movement direction
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, speed * 5f); // Adjust rotation speed multiplier if needed
            }
        }
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

    private bool CheckIfMovingBackwards(Vector3 targetVector)
    {
        // Ray from camera to mouse
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        // Calculate the point on the player's Y-plane that the mouse points to
        float playerY = transform.position.y;
        float distanceToYPlane = (playerY - ray.origin.y) / ray.direction.y;
        Vector3 mousePositionInWorld = ray.origin + ray.direction * distanceToYPlane;

        // Direction from player to mouse
        Vector3 directionToMouse = (mousePositionInWorld - transform.position).normalized;

        // Check the dot product between movement direction and direction to mouse
        // If the dot product is negative, the player is moving in the opposite direction from the mouse
        float dot = Vector3.Dot(targetVector.normalized, directionToMouse);

        return dot < 0;  // Negative dot means moving backwards
    }
}
