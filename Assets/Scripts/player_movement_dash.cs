using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class TeleportSlash : MonoBehaviour
{
    public float teleportDistance = 5f;    // Maximum distance of the teleport
    public int damage = 10;                // Damage dealt to enemies
    public LayerMask enemyLayer;           // LayerMask to identify enemies
    public GameObject linePrefab;          // Prefab for the line renderer

    private CharacterController _characterController; // Reference to the CharacterController

    public int dashAmount = 5;                // Maximum number of dashes
    public int currentDashAmount = 5;         // Current available dashes
    public float dashCooldown = 1f;           // Time in seconds before a single dash is reloaded
    private bool isReloadingDash = false;     // Flag to prevent multiple reload coroutines

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
            // Check if the player has dashes available
            if (currentDashAmount > 0)
            {
                Teleport();

                //reload the gun when you dash
                Gun gun = GetComponent<Gun>();
                gun.ReloadGun();

                // Decrease the current dash count
                currentDashAmount--;

                // Start reloading dashes if not already doing so
                if (!isReloadingDash)
                {
                    StartCoroutine(ReloadDashesCoroutine());
                }

                // Trigger the camera dashing sequence or extend the dash duration
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

                if (cameraFollow.isDashing)
                {
                    // If the camera is already in dashing mode, extend the hold time
                    cameraFollow.ExtendDash(2f); // Extend dash by 2 seconds
                }
                else
                {
                    // Start a new dash sequence with 4 seconds duration
                    StartCoroutine(cameraFollow.DashingSequence(3f));
                }
            }
            else
            {
                // Optionally, play a sound or show feedback indicating no dashes are available
                Debug.Log("No dashes available!");
            }
        }
    }

    IEnumerator ReloadDashesCoroutine()
    {
        isReloadingDash = true;
        while (currentDashAmount < dashAmount)
        {
            yield return new WaitForSeconds(dashCooldown);
            ReloadDashes(1);
        }
        isReloadingDash = false;
    }

    void ReloadDashes(int amount)
    {
        currentDashAmount = Mathf.Min(currentDashAmount + amount, dashAmount);
        // Optionally, update UI or give player feedback on dash reload
    }

    void Teleport()
    {
        if (_characterController == null)
        {
            return; // If no CharacterController, exit early to avoid errors
        }

        // Determine the teleport direction based on mouse position
        Vector3 teleportDirection = GetMouseDirection();

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

    // Function to get the direction from the player to the mouse cursor in world space
    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position

        // Assume the player is on a flat plane (XZ plane)
        Plane plane = new Plane(Vector3.up, transform.position);
        float distanceToPlane;

        if (plane.Raycast(ray, out distanceToPlane))
        {
            Vector3 targetPoint = ray.GetPoint(distanceToPlane); // Get the point on the plane where the ray hits
            Vector3 direction = (targetPoint - transform.position).normalized; // Calculate the direction from the player to the mouse
            return direction;
        }

        return transform.forward; // Fallback to forward if something goes wrong
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
