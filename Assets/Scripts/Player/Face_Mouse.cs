using UnityEngine;

public class CharacterFacing : MonoBehaviour
{
    [SerializeField]
    public bool RotateTowardMouse = true; // Whether the character should rotate towards the mouse position

    [SerializeField]
    private Camera Camera; // Reference to the camera used to calculate mouse position

    [SerializeField]
    private float maxArmAngle = 50f; // Maximum angle the arms can rotate

    [SerializeField]
    private float ikFloats = 0.1f; // how much the arms want to aim 

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (Camera == null)
        {
            Camera = Camera.main; // Assign the main camera if none is assigned
        }

    }

    // OnAnimatorIK is used to dynamically position and rotate the hands using IK
    void OnAnimatorIK(int layerIndex)
    {
        if (RotateTowardMouse && animator)
        {
            // Cast a ray from the camera to the mouse position
            InitialTurnAround();
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            float characterY = transform.position.y; // Get the Y height of the character

            // Calculate how far along the ray we need to go to reach the player's Y-plane
            if (ray.direction.y != 0) // Prevent division by zero
            {
                // Calculate the distance along the ray to intersect with the character's Y plane
                float distanceToYPlane = (characterY - ray.origin.y) / ray.direction.y;

                // Calculate the point where the ray intersects with the player's Y plane
                Vector3 targetPoint = ray.origin + ray.direction * distanceToYPlane;

                // Lock the target's Y position to match the character's Y level
                targetPoint.y = characterY;

                // Calculate the direction from the character to the target (XZ plane)
                Vector3 directionToTarget = targetPoint - transform.position;
                directionToTarget.y = 0; // Keep it on the XZ plane

                // Calculate the angle between the character's forward direction and the direction to the target
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                // If the angle exceeds the maximum allowed, clamp it
                if (angleToTarget <= maxArmAngle)
                {
                    // Apply IK to both hands using ikFloats for weights
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikFloats);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikFloats);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, targetPoint);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(directionToTarget));

                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikFloats);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikFloats);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, targetPoint);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(directionToTarget));
                }
                else
                {
                    // Clamp the arm rotation at the max angle
                    Vector3 clampedDirection = Vector3.RotateTowards(transform.forward, directionToTarget, Mathf.Deg2Rad * maxArmAngle, 0f);

                    // Apply IK to both hands but clamp the rotation to the max allowed angle
                    Vector3 clampedTarget = transform.position + clampedDirection;
                    clampedTarget.y = characterY; // Keep the Y position clamped to the character's Y level

                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikFloats);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikFloats);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, clampedTarget);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(clampedDirection));

                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikFloats);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikFloats);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, clampedTarget);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(clampedDirection));
                }
            }
        }
    }
    private void InitialTurnAround()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position); // Define a plane at the player's Y level
        float rayDistance;

        // If the ray hits the plane, we get the point where it hits
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance); // Get the point where the ray hits the plane

            // Calculate the direction from the character to the target point
            Vector3 directionToTarget = targetPoint - transform.position;
            directionToTarget.y = 0; // Keep the rotation on the XZ plane

            // Rotate the character to face the target point
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }
}
