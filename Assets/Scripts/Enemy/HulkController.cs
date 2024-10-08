
using System.Collections;
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
    }
    public void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // if only just entered fight state, cooldown before attacking
            isCooledDown = false;
            StartCoroutine(Cooldown());
            
            GetComponent<NavMeshAgent>().stoppingDistance = attackRange*attackOffsetMuliplier;
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
        GetComponent<NavMeshAgent>().enabled = false;

        // Disable the collider
        GetComponent<Collider>().enabled = false;

        // Trigger the death animation
        _anim.SetTrigger("goDead");
    }
    protected override void Update()
    {
        base.Update();

        if (!GetComponent<NavMeshAgent>().isStopped) {
            _anim.SetFloat("locomotion", 1f);
        }
        if (isAttacking || enemyState == EnemyState.Patrol) {
            _anim.SetFloat("locomotion", 0f);
        }
    
    }
    public override void Attack()
    {
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
        while (true)
        {
            yield return new WaitUntil(() => !isAttacking);

            float distanceToPlayer = DistanceIgnoreY(transform.position, playerPosition);

            if (distanceToPlayer > attackRange*attackOffsetMuliplier) {
                GetComponent<NavMeshAgent>().isStopped = false;
                GetComponent<NavMeshAgent>().SetDestination(playerPosition);
            }
            else {
                GetComponent<NavMeshAgent>().isStopped = true;
                StartCoroutine(UpdateFacingDirection());
            }
            
            yield return new WaitForSeconds(followUpdateRate);
        }
    }
    IEnumerator UpdateFacingDirection () {
        Vector3 direction = (playerPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Check facing within 1 degree of player
        while (Quaternion.Angle(transform.rotation, targetRotation) > 10f)
        {   
            yield return new WaitUntil(() => !isAttacking);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            // map angle to 1 to 0, 1 being 180 degrees and 0 being 0 degrees
            float animationSpeed = Mathf.Clamp01(angle / 90 + 0.2f);

            // Update the direction to face the player
            direction = (playerPosition - transform.position).normalized;
            
            // Calculate the rotation towards the player
            targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target rotation over time
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnAndFaceSpeed);

            // Wait for the next frame before continuing the loop
            yield return null;
        }
    }
    
}
