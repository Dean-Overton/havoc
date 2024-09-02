using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;  // Character's orientation
    private Rigidbody rb;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("Camera Effects")]
    public camera_controller cam;
    public float dashFov = 85f;

    [Header("Settings")]
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVelocity = true;

    [Header("Cooldown")]
    public float dashCooldown;
    private float dashCooldownTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private Vector3 delayedForceToApply;
    private bool isDashing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0)
        {
            Dash();
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        dashCooldownTimer = dashCooldown;
        isDashing = true;

        cam.DoFov(dashFov);

        // Always dash in the direction the character is facing (orientation)
        Vector3 direction = GetDirection(orientation);
        delayedForceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        if (resetVelocity)
        {
            rb.velocity = Vector3.zero;
        }

        Invoke(nameof(ApplyDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ApplyDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);

        // Limit vertical speed during dash if necessary
        if (rb.velocity.y > maxDashYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxDashYSpeed, rb.velocity.z);
        }
    }

    private void ResetDash()
    {
        isDashing = false;

        cam.DoFov(85f);

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    private Vector3 GetDirection(Transform forwardTransform)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = forwardTransform.forward * verticalInput + forwardTransform.right * horizontalInput;

        if (!allowAllDirections || (horizontalInput == 0 && verticalInput == 0))
        {
            direction = forwardTransform.forward;
        }

        return direction.normalized;
    }
}
