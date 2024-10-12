using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player to follow
    public float followSpeed = 5f; // Speed of following (used when not panning)
    public float panningFollowSpeed = 15f; // Speed of following during panning (used when panning with the middle mouse button)
    public Vector3 initialOffset = new Vector3(0f, 8f, -9f); // Initial camera follow offset
    public float rotateSpeed = 300f; // Speed of camera panning when using the mouse (affects how quickly the camera rotates horizontally)
    public float zoomSpeed = 2f; // Speed of zooming in and out
    public float minZoom = 5f; // Minimum zoom level
    public float maxZoom = 20f; // Maximum zoom level
    private Vector3 currentOffset; // To track the dynamically updated offset for panning

    void Start()
    {
        // Initialize the current offset to its initial value
        currentOffset = initialOffset;
    }

    void LateUpdate()
    {
        if (target == null) return; // Skip following behavior if no target

        HandlePanning(); // Handle mouse panning left and right
        HandleZooming(); // Handle zooming in and out

        float currentFollowSpeed = Input.GetMouseButton(2) ? panningFollowSpeed : followSpeed;

        // Smooth camera follow behavior
        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, currentFollowSpeed * Time.deltaTime);

        // Smoothly rotate to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentFollowSpeed * Time.deltaTime);
    }

    // Handle left and right panning using mouse input (horizontal movement)
    private void HandlePanning()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button held
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

            // Rotate the current offset around the target's position to pan the camera
            Quaternion rotation = Quaternion.AngleAxis(mouseX, Vector3.up);
            currentOffset = rotation * currentOffset;
        }
    }

    // Handle zooming in and out using the mouse scroll wheel
    private void HandleZooming()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollInput * zoomSpeed;

        // Adjust the offset's magnitude based on the zoom input
        currentOffset = Vector3.ClampMagnitude(currentOffset * (1 - zoomAmount), maxZoom);
        currentOffset = currentOffset.normalized * Mathf.Clamp(currentOffset.magnitude, minZoom, maxZoom);
    }
}