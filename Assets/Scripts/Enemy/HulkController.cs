
using System.Collections;
using UnityEngine;

public class HulkController : EnemyController
{
    [Header("Movement")]
    [SerializeField] private float turnAndFaceSpeed = 5f;
    [SerializeField] private float followUpdateRate = 2f;
    [Tooltip("This multiplies with attakc range to set the stopping distance of the navmesh agent.")]
    [SerializeField] private float attackOffsetMuliplier = 0.8f;

    protected override void Start() {
        base.Start();

        onStateChanged += StateMonitor;
        GetComponent<health_component>().onDeath += OnDeath;

        _navMeshAgent.updateRotation = false;
    }
    public void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // if only just entered fight state, cooldown before attacking
            isCooledDown = false;
            StartCoroutine(Cooldown());
            
            _navMeshAgent.stoppingDistance = attackRange*attackOffsetMuliplier;
            StartCoroutine(UpdateTrackingPosition());
        }
    }
    void OnDeath()
    {
        //Debug.Log("hulk died earleir than expexted");
        // Stop all coroutines
        StopAllCoroutines();

        // Disable the navmesh agent
        enemyState = EnemyState.Dead;

        // Disable the collider
        GetComponent<Collider>().enabled = false;

        // Trigger the death animation
        _anim.SetTrigger("goDead");
    }
    protected override void Update()
    {
        base.Update();

        if (!_navMeshAgent.isStopped) {
            _anim.SetFloat("locomotion", 1f);
        }
        if (isAttacking || enemyState == EnemyState.Patrol || enemyState == EnemyState.Dead) {
            _anim.SetFloat("locomotion", 0f);
        }

        NavMeshFacingUpdate();
    }
    private void NavMeshFacingUpdate() {
        if (_navMeshAgent.hasPath) {            
            Vector3 direction = _navMeshAgent.desiredVelocity.normalized;

            // Optionally, rotate the character to face the direction of movement
            if (direction != Vector3.zero)
            {
                turnDirection = direction;

                if (turnAndFaceCoroutine == null && !CheckFacing(transform.position + direction)) {
                    turnAndFaceCoroutine = StartCoroutine(TurnAndFace(transform.position + direction));
                }
            }
        }
    }
    public override void Attack()
    {
        _anim.SetFloat("locomotion", 0f);
        
        if(!isCooledDown || isAttacking) 
            return;
        
        isAttacking = true;
        isCooledDown = false;
        StartCoroutine(SwipeAttack());
    }
    IEnumerator SwipeAttack()
    {
        // Wait for turning and facing to finish
        yield return new WaitUntil(() => turnAndFaceCoroutine == null);

        // Play the attack animation
        string[] attackTriggers = new string[] { "swing", "punch" };

        string attackTrigger = attackTriggers[Random.Range(0, attackTriggers.Length)];
        Debug.Log("Trigger attack animation: " + attackTrigger);
        _anim.SetTrigger(attackTrigger);

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(attackCooldown);

        // Set the attacking state to false
        isAttacking = false;

        // Cooldown before attacking again
        StartCoroutine(Cooldown());
    }
    IEnumerator UpdateTrackingPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isAttacking);

            float distanceToPlayer = DistanceIgnoreY(transform.position, playerPosition);

            if (distanceToPlayer > attackRange*attackOffsetMuliplier) {
                if (_navMeshAgent.destination != playerPosition) {
                    _navMeshAgent.SetDestination(playerPosition);

                    // Update facing direction
                    // Set the target for the player
                    Vector3 target = playerPosition;
                    // Replace target with the next point in the path if it exists
                    if (_navMeshAgent.path.corners.Length > 1)
                        target = _navMeshAgent.path.corners[1];

                    // Stop before turning
                    _navMeshAgent.isStopped = true;
                    // Turn and face the next point in the path
                    turnAndFaceCoroutine = StartCoroutine(TurnAndFace(target));
                    yield return new WaitUntil(() => turnAndFaceCoroutine == null);
                    
                    _navMeshAgent.isStopped = false;
                }
            }
            else {
                _navMeshAgent.isStopped = true;

                turnAndFaceCoroutine = StartCoroutine(TurnAndFace(playerPosition));
            }
            
            yield return new WaitForSeconds(followUpdateRate);
        }
    }
    private Coroutine _turnAndFaceCoroutine = null;

    // Property to encapsulate coroutine management
    private Coroutine turnAndFaceCoroutine
    {
        get { return _turnAndFaceCoroutine; }
        set
        {
            // Stop the current coroutine if it's already running
            if (_turnAndFaceCoroutine != null)
            {
                StopCoroutine(_turnAndFaceCoroutine);
            }

            // Start the new coroutine if the value is not null
            _turnAndFaceCoroutine = value;
        }
    }
    bool CheckFacing (Vector3 targetPosition) {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        return Mathf.Abs(angle) < 1f;
    }
    [Tooltip("The angle at which the enemy will play an animation to turn.")]
    [SerializeField] private float _turnAnimationAngle = 70f;
    IEnumerator TurnAndFace(Vector3 targetPosition, float turnAndFaceSpeedModifier = 1f)
    {        
        // Calculate direction to the target
        Vector3 directionToTarget = targetPosition - transform.position;

        // if already facing target position, return
        float initialAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        if (Mathf.Abs(initialAngle) < 1f) {
            yield break;
        }

        // float initialAngle = Vector3.Cross(transform.forward, targetRotation * Vector3.forward).y;

        // TURNING ANIMATION
        // Check angle is greater than _turnAnimationAngle and turn right animation if so
        if (initialAngle > 70f && initialAngle < 180f) {
            _anim.SetTrigger("turnRight");
            if (!_navMeshAgent.isStopped) {
                _navMeshAgent.isStopped = true;
            }
        }
        // Check angle is less than -_turnAnimationAngle and turn left animation if so
        else if (initialAngle < -70f && initialAngle > -180f) {
            _anim.SetTrigger("turnLeft");
            if (!_navMeshAgent.isStopped) {
                _navMeshAgent.isStopped = true;
            }
        }

        // ROTATING TRANSFORM
        float angle = initialAngle;
        // Convert vector3 angle to quaternion rotation
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        float time = 0f;
        while (Mathf.Abs(angle) > 1f)
        {
            // Smoothly rotate towards the target rotation over time
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            
            time += Time.deltaTime * turnAndFaceSpeed * turnAndFaceSpeedModifier;

            // Get the angle between the current rotation and the target rotation
            angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
            yield return null;
        }
        
        // Wait until animation finished
        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        if (initialAngle > 70f && initialAngle < 180f) {
            if (stateInfo.IsName("rightTurn")) {
                yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
            }
        }
        else if (initialAngle < -70f && initialAngle > -180f) {
            if (stateInfo.IsName("leftTurn")) {
                yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
            }
        }
        // Signal that the coroutine has finished
        turnAndFaceCoroutine = null;
    }
    public override void DealDamage(int thisDamage = 20)
    {
        // do phyics collision in front of plant and check if player is hit
        // if player is hit, apply damage

        Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                hitCollider.GetComponent<health_component>().ReduceCurrentHealth(thisDamage);

                break; // Only hit the player once
            }
        }
    
    }
    Vector3 turnDirection;
    private void OnDrawGizmos() {
        // Draw line to show direction turning to
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + turnDirection);
    }
}
