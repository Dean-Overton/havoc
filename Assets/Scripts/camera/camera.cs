using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player to follow
    public float followSpeed = 2f; // Speed of following
    public Vector3 initialOffset = new Vector3(0f, 8f, -9f); // Initial camera follow offset
    public Vector3 initialPanBackVector = new Vector3(0f, 15f, -19f); // Initial offset for aerial view during dash
    public float rotateSpeed = 200f; // Speed of camera rotation when panning with the mouse
    public bool isDashing = false; // Flag to check if in dashing sequence
    private bool canDash = true; // Prevent overlapping dashes
    private float dashHoldTimer = 0f; // Timer to hold the aerial view during dashing
    private Vector3 currentOffset; // To track the dynamically updated offset for panning and dashing
    private Vector3 currentPanBackVector; // Dynamic pan back vector that adjusts with panning

    void Start()
    {
        // Initialize the current offset and pan back vector to their initial values
        currentOffset = initialOffset;
        currentPanBackVector = initialPanBackVector;
    }

    void LateUpdate()
    {
        if (isDashing || target == null) return; // Skip following behavior during dash

        HandlePanning(); // Handle mouse panning left and right

        // Smooth camera follow behavior
        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    // Handle left and right panning using mouse input (horizontal movement)
    private void HandlePanning()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button held
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

            // Rotate the current offset around the Y-axis to pan the camera
            Quaternion rotation = Quaternion.Euler(0, mouseX, 0);
            currentOffset = rotation * currentOffset;

            // Adjust the panBackVector similarly to ensure proper zoom-out direction
            currentPanBackVector = rotation * currentPanBackVector;

            // Ensure the offset values don't multiply by zero (if they're too small)
            if (Mathf.Abs(currentOffset.x) < 0.01f) currentOffset.x = 0.01f;
            if (Mathf.Abs(currentOffset.z) < 0.01f) currentOffset.z = 0.01f;

            // Ensure the panBackVector values don't multiply by zero (if they're too small)
            if (Mathf.Abs(currentPanBackVector.x) < 0.01f) currentPanBackVector.x = 0.01f;
            if (Mathf.Abs(currentPanBackVector.z) < 0.01f) currentPanBackVector.z = 0.01f;
        }
    }

    // Coroutine to handle the dashing sequence (aerial view) and return to normal view
    public IEnumerator DashingSequence(float dashDuration)
    {
        if (!canDash) yield break; // Prevent overlapping dashes
        canDash = false; // Disable additional dashes during this sequence

        isDashing = true; // Mark that the camera is in dashing mode

        // Phase 1: Transition to aerial view
        Vector3 startPosition = transform.position;
        Vector3 aerialPosition = target.position + currentPanBackVector; // Use the dynamic panBackVector
        float elapsedTime = 0f;

        // Smoothly move to the aerial view
        while (elapsedTime < dashDuration / 4f)
        {
            transform.position = Vector3.Lerp(startPosition, aerialPosition, elapsedTime / (dashDuration / 4f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final aerial position
        transform.position = aerialPosition;

        // Phase 2: Hold the aerial view for the duration of the dash
        dashHoldTimer = dashDuration; // Initialize the hold timer

        while (dashHoldTimer > 0f)
        {
            dashHoldTimer -= Time.deltaTime;
            yield return null;
        }

        // Phase 3: Return to panned follow position
        elapsedTime = 0f;
        while (elapsedTime < dashDuration / 4f)
        {
            transform.position = Vector3.Lerp(aerialPosition, target.position + currentOffset, elapsedTime / (dashDuration / 4f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position to avoid floating-point errors
        transform.position = target.position + currentOffset;

        isDashing = false; // End dashing mode, resume normal camera behavior
        canDash = true; // Re-enable dashing
    }

    public void ExtendDash(float extraTime)
    {
        // Add extra time to the dash hold timer to prevent the camera from zooming back in too early
        dashHoldTimer = Mathf.Max(dashHoldTimer, extraTime);
    }
}
