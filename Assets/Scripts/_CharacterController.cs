using UnityEngine;

public class _CharacterController : MonoBehaviour   // This was renamed as it uses the same name as a Unity built-in component, it will not work otherwise
{
    public float moveSpeed = 8f;
    public bool isRun = false;

    private Rigidbody rb;
    //private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        rb.velocity = Vector3.zero; 
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // control movement
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        movement *= moveSpeed;
        rb.velocity = movement; // Use Rigidbody's velocity property to control movement

        /*
        // control player facing
        if (movement.magnitude > 0.1f) // rotate when moving
        {
            transform.rotation = Quaternion.LookRotation(movement);
            isRun = true;
        }
        else
        {
            isRun = false;
        }
        */

        /*
        // trigger animation
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetBool("isRun", isRun);
        */
    }
}