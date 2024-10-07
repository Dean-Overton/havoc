using UnityEngine;
using System.Collections;

public class TeleportSlash : MonoBehaviour
{
    public float teleportDistance = 5f;    // Maximum distance of the teleport
    public int damage = 10;                // Damage dealt to enemies
    public LayerMask enemyLayer;           // LayerMask to identify enemies
    public LayerMask barrierLayer;         // LayerMask to identify barriers
    public GameObject linePrefab;          // Prefab for the line renderer

    private CharacterController _characterController; // Reference to the CharacterController
    private Collider _collider;                       // Reference to the collider for disabling during teleport

    public int dashAmount = 5;                // Maximum number of dashes
    public int currentDashAmount = 5;         // Current available dashes
    public float dashCooldown = 1f;           // Time in seconds before a single dash is reloaded
    private bool isReloadingDash = false;     // Flag to prevent multiple reload coroutines

    void Start()
    {
        // Get the CharacterController component
        _characterController = GetComponent<CharacterController>();
        _collider = GetComponent<Collider>();

        // Ensure components exist
        if (_characterController == null)
        {
            Debug.LogError("CharacterController not found on " + gameObject.name);
        }
        if (_collider == null)
        {
            Debug.LogError("Collider not found on " + gameObject.name);
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
                StartCoroutine(Teleport());

                // Reload the gun when you dash
                Gun gun = GetComponent<Gun>();
                if (gun != null)
                {
                    gun.ReloadGun();
                }

                // Decrease the current dash count
                currentDashAmount--;

                // Start reloading dashes if not already doing so
                if (!isReloadingDash)
                {
                    StartCoroutine(ReloadDashesCoroutine());
                }

                // Trigger the camera dashing sequence or extend the dash duration
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

                if (cameraFollow != null && cameraFollow.isDashing)
                {
                    // If the camera is already in dashing mode, extend the hold time
                    cameraFollow.ExtendDash(2f); // Extend dash by 2 seconds
                }
                else if (cameraFollow != null)
                {
                    // Start a new dash sequence with 4 seconds duration
                    StartCoroutine(cameraFollow.DashingSequence(3f));
                }
            }
            else
            {
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
    }

    IEnumerator Teleport()
    {
        if (_characterController == null || _collider == null)
        {
            yield break; // If necessary components are missing, exit early
        }

        Vector3 teleportDirection = GetMouseDirection();

        // Calculate the intended teleport position
        Vector3 targetPosition = transform.position + teleportDirection * teleportDistance;

        // Check for barriers in the path using a raycast
        RaycastHit barrierHit;
        if (Physics.Raycast(transform.position, teleportDirection, out barrierHit, teleportDistance, barrierLayer))
        {
            // If a barrier is hit, set the target position to the hit point
            targetPosition = barrierHit.point;
            Debug.Log("Hit barrier: " + barrierHit.collider.gameObject.name);
        }

        // Disable collision temporarily for a smooth teleport
        _collider.enabled = false;

        // Draw the ray for debugging purposes (shows the teleport direction in the Scene view)
        Debug.DrawRay(transform.position, teleportDirection * teleportDistance, Color.red, 2f); // Draws for 2 seconds

        // Define a radius for the sphere cast to make the detection area larger
        float sphereRadius = 1.0f;

        // Perform a sphere cast to deal damage to any enemies in the teleport path
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, teleportDirection, teleportDistance, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            health_component enemyHealth = hit.collider.GetComponent<health_component>();
            if (enemyHealth != null)
            {
                enemyHealth.ReduceCurrentHealth(damage);
                Debug.Log("Dealt " + damage + " damage to " + hit.collider.gameObject.name);
            }
        }

        // Move the character instantly to the target position
        transform.position = targetPosition;

        // Spawn the line at the start and end points to visualize the teleport
        SpawnTeleportLine(transform.position - teleportDirection * teleportDistance, transform.position);

        // Wait for the frame to complete to ensure the teleport is finished
        yield return null;

        // Re-enable collision after teleportation
        _collider.enabled = true;

        Debug.Log("Teleported to: " + targetPosition);
    }

    // Function to get the direction from the player to the mouse cursor in world space
    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Assume the player is on a flat plane (XZ plane)
        Plane plane = new Plane(Vector3.up, transform.position);
        float distanceToPlane;

        if (plane.Raycast(ray, out distanceToPlane))
        {
            Vector3 targetPoint = ray.GetPoint(distanceToPlane);
            Vector3 direction = (targetPoint - transform.position).normalized;
            return direction;
        }

        return transform.forward; // Fallback to forward if something goes wrong
    }

    void SpawnTeleportLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = Instantiate(linePrefab, start, Quaternion.identity);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // Customize line appearance (width, color, etc.)
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material.color = Color.cyan;

            // Destroy the line after a short duration
            Destroy(lineObject, 0.5f);
        }
        else
        {
            Debug.LogWarning("LineRenderer component not found on linePrefab.");
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Define the starting point of the sphere cast and the teleport direction
        Vector3 startPoint = transform.position;
        Vector3 teleportDirection = GetMouseDirection();

        // Define the radius of the sphere cast
        float sphereRadius = 1.0f;

        // Set the color for the gizmos
        Gizmos.color = Color.yellow;

        // Draw the starting point of the sphere cast
        Gizmos.DrawWireSphere(startPoint, sphereRadius);

        // Draw spheres along the path to visualize the entire sphere cast path
        int segments = 10; // Number of segments to visualize along the path
        float segmentLength = teleportDistance / segments;

        for (int i = 0; i <= segments; i++)
        {
            Vector3 segmentPosition = startPoint + teleportDirection * segmentLength * i;
            Gizmos.DrawWireSphere(segmentPosition, sphereRadius);
        }
    }
}
