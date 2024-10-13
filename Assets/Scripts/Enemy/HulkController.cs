
using System.Collections;
//using UnityEditor.Build;
using UnityEngine;

public class HulkController : EnemyController
{
    protected override void Start() {
        base.Start();

        GetComponent<health_component>().onDeath += OnDeath;

        StartCoroutine(UpdateTrackingPosition());
    }
    void OnDeath()
    {
        //Debug.Log("hulk died earleir than expexted");
        // Stop all coroutines
        StopAllCoroutines();

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

    [Tooltip("The angle at which the enemy will play an animation to turn.")]
    [SerializeField] private float _turnAnimationAngle = 70f;
    protected override IEnumerator TurnAndFace(Vector3 targetPosition, float turnAndFaceSpeedModifier = 1f)
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
        if (initialAngle > _turnAnimationAngle && initialAngle < 180f) {
            _anim.SetTrigger("turnRight");
            if (!_navMeshAgent.isStopped) {
                _navMeshAgent.isStopped = true;
            }
        }
        // Check angle is less than -_turnAnimationAngle and turn left animation if so
        else if (initialAngle < -_turnAnimationAngle && initialAngle > -180f) {
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
    private void OnDrawGizmos() {
        // Draw line to show direction turning to
        Gizmos.color = Color.red;
    }
}
