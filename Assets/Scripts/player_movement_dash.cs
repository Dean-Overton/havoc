using UnityEngine;

public class TeleportSlash : MonoBehaviour
{
    public float teleportDistance = 5f;    // Distance of the teleport
    public int damage = 10;                // Damage dealt to enemies
    public LayerMask enemyLayer;           // LayerMask to identify enemies
    public GameObject linePrefab;          // Prefab for the line renderer

    private CharacterController _characterController; // Reference to the CharacterController

    void Start()
    {
        // Get the CharacterController component
        _characterController = GetComponent<CharacterController>();

        // Ensure the CharacterController exists
        if (_characterController == null)
        {
            Debug.LogError("CharacterController not found on " + gameObject.name);
        }
    }

    void Update()
    {
        // Check for the left mouse button press to trigger teleport
        if (Input.GetMouseButtonDown(0))
        {
            Teleport();

            // Trigger the camera dashing sequence or extend the dash duration
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

            if (cameraFollow.isDashing)
            {
                // If the camera is already in dashing mode, extend the hold time
                cameraFollow.ExtendDash(2f); // Extend dash by 2 seconds
            }
            else
            {
                // Start a new dash sequence with 2 seconds duration
                StartCoroutine(cameraFollow.DashingSequence(2f));
            }
        }
    }

    void Teleport()
    {
        if (_characterController == null)
        {
            return; // If no CharacterController, exit early to avoid errors
        }

        // Determine teleport direction based on character's facing direction
        Vector3 teleportDirection = transform.forward;

        // Perform a raycast to check for enemies in the teleport path
        RaycastHit[] hits = Physics.RaycastAll(transform.position, teleportDirection, teleportDistance, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Deal damage to enemies within the teleport path
                hit.collider.GetComponent<health_component>()?.ReduceCurrentHealth(damage);
            }
        }

        // Calculate the teleport end position
        Vector3 teleportEndPosition = transform.position + teleportDirection * teleportDistance;

        // Check for obstacles in the teleport path using raycasting
        RaycastHit teleportHit;
        if (Physics.Raycast(transform.position, teleportDirection, out teleportHit, teleportDistance))
        {
            // If there's an obstacle, stop at the obstacle point
            teleportEndPosition = teleportHit.point;
        }

        // Spawn the line at the start and end points to visualize the teleport
        SpawnTeleportLine(transform.position, teleportEndPosition);

        // Calculate the movement vector from current position to teleport end position
        Vector3 moveDirection = teleportEndPosition - transform.position;

        // Move the character using the CharacterController
        _characterController.Move(moveDirection);
    }

    void SpawnTeleportLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(linePrefab, start, Quaternion.identity);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        // Set the line's start and end positions
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Customize line appearance (width, color, etc.)
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.cyan;  // Bright color for teleport

        // Destroy the line after a short duration
        Destroy(lineObject, 0.5f);
    }
}