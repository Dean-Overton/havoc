using UnityEngine;

public class camera_controller : MonoBehaviour
{
    public Transform target; // The target object that needs to follow
    public float followSpeed = 2f;
    public Vector3 offset = new Vector3(0f, 30f, -30f); // The camera's relative target is offset

    private Camera cam;
    private float targetFov;
    private float fovTransitionSpeed = 5f;  // Speed of FOV change

    private void Start()
    {
        // Assign the Transform of the character game object to the target attribute of the CameraFollow script
        target = GameObject.FindGameObjectWithTag("Player").transform;

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        targetFov = cam.fieldOfView;
    }

    private void LateUpdate()
    {
        // Calculate the position of the camera
        Vector3 desiredPosition = target.position + offset;

        // Use lerp to smooth the camera between two frames
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Let the camera look at the target all the time
        transform.LookAt(target);

        // Smoothly transition to the target FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovTransitionSpeed);
    }

    // Method to adjust the FOV during gameplay
    public void DoFov(float newFov)
    {
        targetFov = newFov;
    }
}
