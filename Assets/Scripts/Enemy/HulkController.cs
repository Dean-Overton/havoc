
using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

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
        else if(newState == EnemyState.Patrol)
        {
            // 
        }
    }
    void OnDeath()
    {
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
        NavMeshTurnUpdate();

        while (true)
        {
            yield return new WaitUntil(() => !isAttacking);

            float distanceToPlayer = DistanceIgnoreY(transform.position, playerPosition);

            if (distanceToPlayer > attackRange*attackOffsetMuliplier) {
                if (_navMeshAgent.destination != playerPosition) {
                    _navMeshAgent.SetDestination(playerPosition);

                    // When updating target, check path is different and turn to face force point in path
                    // before moving

                    // Get the direction to the next point in the path
                    Vector3 direction;
                    if (_navMeshAgent.path.corners.Length > 1) { 
                        // Set the direction to the next point in the path
                        direction = (_navMeshAgent.path.corners[1] - transform.position).normalized;
                    } else {
                        // Otherwise, set the direction to the player if no corner exist in path
                        direction = (playerPosition - transform.position).normalized;
                    }
                    // Get the rotation towards the next point in the path
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    // Turn and face the next point in the path
                    StartCoroutine(TurnAndFace(targetRotation));
                    // Wait until the rotation is within 1 degree of the target rotation
                    yield return new WaitUntil(() => Quaternion.Angle(transform.rotation, targetRotation) < 1f);
                    //Start moving
                    _navMeshAgent.isStopped = false;
                }
            }
            else {
                _navMeshAgent.isStopped = true;

                Quaternion targetRotation = Quaternion.LookRotation((playerPosition - transform.position).normalized);
                StartCoroutine(TurnAndFace(targetRotation));
            }
            
            yield return new WaitForSeconds(followUpdateRate);
        }
    }
    [Tooltip("The angle at which the enemy will play an animation to turn.")]
    [SerializeField] private float _turnAnimationAngle = 70f;
    IEnumerator TurnAndFace(Quaternion targetRotation, float turnAndFaceSpeedModifier = 1f)
    {
        // Stop multiple calls to this coroutine
        StopCoroutine("TurnAndFace");

        float initialAngle = Vector3.Cross(transform.forward, targetRotation * Vector3.forward).y;
        bool wasStopped = _navMeshAgent.isStopped;

        // TURNING ANIMATION
        // Check angle is greater than _turnAnimationAngle and turn right animation if so
        if (initialAngle > 70f && initialAngle < 180f) {
            _anim.SetTrigger("turnRight");
            if (!wasStopped) {
                _navMeshAgent.isStopped = true;
            }
        }
        // Check angle is less than -_turnAnimationAngle and turn left animation if so
        else if (initialAngle < -70f && initialAngle > -180f) {
            _anim.SetTrigger("turnLeft");
            if (!wasStopped) {
                _navMeshAgent.isStopped = true;
            }
        }

        // ROTATING TRANSFORM
        // Check facing within 1 degree of player
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.3f)
        {
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            // map angle to 1 to 0, 1 being 180 degrees and 0 being 0 degrees
            // float animationSpeed = Mathf.Clamp01(angle / 90 + 0.2f);

            // Smoothly rotate towards the target rotation over time
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnAndFaceSpeed);

            // Wait for the next frame before continuing the loop
            yield return null;
        }

        // Check if the navmesh agent was stopped before turning and set it back to normal
        if (wasStopped) {
            _navMeshAgent.isStopped = true;
        } else {
            _navMeshAgent.isStopped = false;
        }
    }
    IEnumerator NavMeshTurnUpdate () {
        while (true) {
            yield return new WaitUntil(() => !isAttacking);
            yield return new WaitUntil(() => _navMeshAgent.velocity.sqrMagnitude > 0.1f);

            // Get the direction to the next point in the path
            Vector3 direction = _navMeshAgent.velocity.normalized;
            // Get the rotation towards the next point in the path
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
            if (Quaternion.Angle(transform.rotation, targetRotation) > 1f) {
                // Turn and face the next point in the path
                StartCoroutine(TurnAndFace(targetRotation));
            }
            // Wait until the rotation is within 1 degree of the target rotation
            yield return new WaitUntil(() => Quaternion.Angle(transform.rotation, targetRotation) < 1f);
        }
    }
    
}
