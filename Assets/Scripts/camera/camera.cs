using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player to follow
    public float followSpeed = 2f; // Speed of following
    public Vector3 offset = new Vector3(10f, 5f, -5f); // Normal follow offset
    public Vector3 panBackVector = new Vector3(10f, 15f, -15f); // Offset for aerial view
    public bool isDashing = false; // Flag to check if in dashing sequence
    private bool canDash = true; // Prevent overlapping dashes
    private float dashHoldTimer = 0f; // Timer to hold the aerial view during dashing

    void Start()
    {
        // Automatically assign the player as target if tagged "Player"
        // target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (isDashing || target == null) return; // Skip following behavior during dash

        // Normal follow behavior
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    public IEnumerator DashingSequence(float dashDuration)
    {
        if (!canDash) yield break; // Prevent overlapping dashes
        canDash = false; // Disable additional dashes during this sequence

        isDashing = true; // Mark that the camera is in dashing mode

        // Phase 1: Transition to aerial view
        Vector3 startPosition = transform.position;
        Vector3 aerialPosition = target.position + panBackVector;
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

        // Phase 2: Hold the aerial view for the duration of the dashes
        dashHoldTimer = dashDuration; // Initialize the hold timer

        while (dashHoldTimer > 0f)
        {
            dashHoldTimer -= Time.deltaTime;
            yield return null;
        }

        // Phase 3: Return to normal follow position
        elapsedTime = 0f;
        while (elapsedTime < dashDuration / 4f)
        {
            transform.position = Vector3.Lerp(aerialPosition, target.position + offset, elapsedTime / (dashDuration / 4f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position to avoid floating-point errors
        transform.position = target.position + offset;

        isDashing = false; // End dashing mode, resume normal camera behavior
        canDash = true; // Re-enable dashing
    }

    public void ExtendDash(float extraTime)
    {
        // Add extra time to the dash hold timer to prevent the camera from zooming back in too early
        dashHoldTimer = Mathf.Max(dashHoldTimer, extraTime);
    }
}