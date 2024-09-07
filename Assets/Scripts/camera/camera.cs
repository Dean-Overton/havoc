using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target object that needs to follow
    public float followSpeed = 2f;
    public Vector3 offset = new Vector3(0f, 30f, -30f); // The camera's relative target is offset

    private void LateUpdate()
    {
        // calculate the position of camera
        Vector3 desiredPosition = target.position + offset;

        // Use lerp to smooth the camera between two frames
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Let the camera look at the target all the time
        transform.LookAt(target);
    }
    private void Start()
    {
        // Assign the Transform of the character game object to the target attribute of the CameraFollow script
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}