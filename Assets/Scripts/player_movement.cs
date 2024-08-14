using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float drag = 1f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;

    }

    private void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate the direction of the movement
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Set the velocity directly
        rb.velocity = moveDirection * moveSpeed;
    }
}
