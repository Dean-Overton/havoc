using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    private EnemyController _enemyController;

    [SerializeField]
    [Tooltip("The waypoints the enemy will patrol between")]
    private List<Transform> _patrolWaypoints = new List<Transform>();
    private int _currentWaypoint = 0;

    [SerializeField] private float _defaultMovementSpeed = 2f; // Speed at which the character moves
    [SerializeField] private float _runMultiplier = 2f; // Speed Multiplier for run speed
    [SerializeField] private float _jumpTime = 0.5f; // Speed Multiplier for jump speed

    [SerializeField]
    private float _rotationSpeed = 5f; // Speed at which the character rotates

    private Rigidbody _rb;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyController.enemyState)
        {
            case EnemyState.Patrol:
                PatrolMovement();
                break;
            case EnemyState.Fight:
                FightMovement();
                break;
        }
    }
    void FightMovement()
    {
        // Choose random position with _enemyController.attackRange around the player
        Vector2 _targetPositionInRangeXZ = new Vector2(_enemyController.playerPosition.x, _enemyController.playerPosition.z) + Random.insideUnitCircle * _enemyController.attackRange;
        Vector3 _targetPositionInRange = new Vector3(_targetPositionInRangeXZ.x, transform.position.y, _targetPositionInRangeXZ.y);

        // Check player is in range without checking the y axis
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_enemyController.playerPosition.x, _enemyController.playerPosition.z)) < _enemyController.attackRange)
        {
            // Jump towards the player
            JumpTowardsPosition(_enemyController.playerPosition);

            // Check for movement and set walk animation state
            _enemyController.animationState = AnimationState.Attack;

            // _enemyController.Attack();
        }
        else
        {
            // Move towards the random position
            bool isMoving = MoveTowards(_targetPositionInRange);

            // Check for movement and set walk animation state
            if (isMoving)
                _enemyController.animationState = AnimationState.Run;
            else
                _enemyController.animationState = AnimationState.Idle;
        }
    }
    void PatrolMovement()
    {
        // Move towards the next waypoint
        Vector3 targetPosition = new Vector3(_patrolWaypoints[_currentWaypoint].position.x, transform.position.y, _patrolWaypoints[_currentWaypoint].position.z);

        // Check if the enemy has reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            // Move to the next waypoint
            _currentWaypoint++;
            if (_currentWaypoint >= _patrolWaypoints.Count)
            {
                _currentWaypoint = 0;
            }
        }
        else
        {
            bool isMoving = MoveTowards(targetPosition);

            // Check for movement and set walk animation state
            if (isMoving)
                _enemyController.animationState = AnimationState.Walk;
            else
            {
                _enemyController.animationState = AnimationState.Idle;
            }
        }
    }
    // Jumps towards the target position
    private void JumpTowardsPosition(Vector3 targetPosition)
    {
        Debug.DrawLine(transform.position, targetPosition, Color.red);
        // Quickly move the enemy to the target position
        // Check if nothing is in the way
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetPosition - transform.position, out hit, Vector3.Distance(transform.position, targetPosition)))
        {
            // If something is in the way, don't jump
            if (hit.collider.gameObject != _enemyController.gameObject)
            {
                return;
            }
        }

        // Move towards position over _jumpTime
        float _jumpTimer = 0f;
        // Calculate speed to move at over _jumpTime
        float _jumpSpeed = Vector3.Distance(transform.position, targetPosition) / _jumpTime;
        while (_jumpTimer < _jumpTime)
        {
            // Move towards the target position
            Vector3 movementDirection = targetPosition - transform.position;
            Vector3 movement = movementDirection.normalized * _jumpSpeed * Time.deltaTime;

            // Move the rigidbody
            _rb.MovePosition(transform.position + movement);

            // Rotate the enemy to face the target position
            RotateTowards(targetPosition);

            // Increment the timer
            _jumpTimer += Time.deltaTime;
        }
    }
    private bool MoveTowards(Vector3 targetPosition)
    {
        // Calculate the movement multiplier based on the animation state
        float _movementMultiplier = _enemyController.animationState == AnimationState.Run ? _runMultiplier : 1f;

        // Translate the rigidbody towards the target position
        Vector3 movementDirection = targetPosition - transform.position;
        Vector3 movement = movementDirection.normalized * _defaultMovementSpeed * _movementMultiplier * Time.deltaTime;

        // Move the rigidbody
        _rb.MovePosition(transform.position + movement);

        // Rotate the enemy to face the target position
        RotateTowards(targetPosition);

        // Check if the character is moving by rigidbody in case of collision
        return movementDirection.magnitude > 0;
    }
    private void RotateTowards(Vector3 targetPosition)
    {
        // Calculate the direction to the target
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        // Rotate the enemy to face the target
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);

        // Check if rotation is positive or negative
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        if (angle > 0)
        {
            // Rotate right
            _enemyController.animationState = AnimationState.TurnRight;
        }
        else if (angle < 0)
        {
            // Rotate left
            _enemyController.animationState = AnimationState.TurnLeft;
        }
    }
    
}
