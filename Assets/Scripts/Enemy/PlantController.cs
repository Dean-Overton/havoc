using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// All behaviours for groot plant enemy
public class PlantController : EnemyController
{
    [Space]
    [SerializeField] private int attackSwipeDamage = 20;
    [SerializeField] private float turnAndFaceSpeed = 5f;
    [SerializeField] private float turnAndFaceUpdateTime = 1f;

    [Header("Cast Attack Settings")]
    [SerializeField] private bool enableCastAttack = true;
    [SerializeField] private float castAttackDistance = 2f;
    [SerializeField] private float castChargeTime = 2f;
    
    [SerializeField] private int castDamage = 10;
    [SerializeField] private GameObject castProjectile;
    [SerializeField] private Transform castPoint;
    [Header("Jump Attack Settings")]
    [SerializeField] private bool enableJumpAttack = true;
    [SerializeField] private float jumpAttackDistanceMax = 5f;
    [SerializeField] private float jumpAttackDistanceMin = 3f;
    [SerializeField] private float jumpAttackSpeed = 5f;
    
    [SerializeField] private int jumpDamage = 30;

    [Header("Movement Settings")]
    [SerializeField] bool enablePlayerFollow = true;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float followUpdateRate = 2f;
    [SerializeField] private float stopDistance = 2f;
    
    protected override void Start() {
        base.Start();

        onStateChanged += StateMonitor;
    }
    public void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // Come alive
            _anim.SetTrigger("goAlive");

            // if only just entered fight state, cooldown before attacking
            isCooledDown = false;
            StartCoroutine(Cooldown());

            if (!enablePlayerFollow)
                StartCoroutine(UpdateFacingDirection());
            
            if (enablePlayerFollow) {
                GetComponent<NavMeshAgent>().stoppingDistance = jumpAttackDistanceMin;
                StartCoroutine(UpdateTrackingPosition());
                StartCoroutine(UpdateMovementFacingDirection());
            }
        }
        else if(newState == EnemyState.Patrol)
        {
            // Die
            _anim.SetTrigger("goDead");

            StopCoroutine(UpdateFacingDirection());
        }
    }
    protected override void Update()
    {
        base.Update();
        
        if (enemyState == EnemyState.Fight)
        {
            if (enableCastAttack)
                CastAttackUpdateLogic();

            if (enableJumpAttack)
                JumpAttackUpdateLogic();

            if (!GetComponent<NavMeshAgent>().isStopped && enablePlayerFollow)
                _anim.SetFloat("locomotion", 1f);
            
            if (isAttacking)
                _anim.SetFloat("locomotion", 0f);

        }
    }
    IEnumerator UpdateTrackingPosition()
    {
        while (true)
        {
            float distanceToPlayer = DistanceIgnoreY(transform.position, playerPosition);

            if (distanceToPlayer > stopDistance) {
                GetComponent<NavMeshAgent>().isStopped = false;
                GetComponent<NavMeshAgent>().SetDestination(playerPosition);
            }
            else {
                GetComponent<NavMeshAgent>().isStopped = true;
                StartCoroutine(UpdateMovementFacingDirection());
            }
            
            yield return new WaitForSeconds(followUpdateRate);
        }
    }
    IEnumerator UpdateMovementFacingDirection () {
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
    IEnumerator UpdateFacingDirection()
    {
        while(true)
        {   
            // Calculate rotation to face player
            Vector3 direction = (playerPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Check facing within 1 degree of player
            while (Quaternion.Angle(transform.rotation, targetRotation) > 10f)
            {   
                yield return new WaitUntil(() => !isAttacking);

                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                // map angle to 1 to 0, 1 being 180 degrees and 0 being 0 degrees
                float animationSpeed = Mathf.Clamp01(angle / 90 + 0.2f);
                // Slow tread backwards while turning
                if (GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
                    _anim.SetFloat("locomotion", -animationSpeed);

                // Update the direction to face the player
                direction = (playerPosition - transform.position).normalized;
                
                // Calculate the rotation towards the player
                targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the target rotation over time
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnAndFaceSpeed);

                // Wait for the next frame before continuing the loop
                yield return null;
            }
            _anim.SetFloat("locomotion", 0f);

            yield return new WaitForSeconds(turnAndFaceUpdateTime);
        }
    }
    bool canCastAttack = false;
    public void CastAttackUpdateLogic()
    {
        if(DistanceIgnoreY(transform.position, playerPosition) > castAttackDistance)
        {
            canCastAttack = false;
            return;
        }
        canCastAttack = true;
        Attack();
    }
    public void JumpAttackUpdateLogic()
    {
        if(DistanceIgnoreY(transform.position, playerPosition) > jumpAttackDistanceMax 
        || DistanceIgnoreY(transform.position, playerPosition) < jumpAttackDistanceMin)
        {
            canJumpAttack = false;
            return;
        }

        canJumpAttack = true;
        Attack();
    }
    Dictionary<string, int> defaultAttackWeights = new Dictionary<string, int> {
            {"Swipe", 60},
            {"Cast", 15},
            {"Jump", 35}
        };
    int GetAttackMethod () {
        Dictionary<string, int> attackWeights = new Dictionary<string, int>(defaultAttackWeights);
        if(!canDefaultAttack)
        {
            attackWeights["Swipe"] = 0;
        }
        if(!canCastAttack)
        {
            attackWeights["Cast"] = 0;
        }
        if (!canJumpAttack)
        {
            attackWeights["Jump"] = 0;
        }
        Debug.Log("Attack weights: " + string.Join(", ", attackWeights.Select(x => x.Key + ": " + x.Value).ToArray()));
        int totalWeight = attackWeights.Values.Sum();
        int randomWeight = Random.Range(0, totalWeight);
        for (int i = 0; i < attackWeights.Count; i++)
        {
            randomWeight -= attackWeights.Values.ElementAt(i);
            if (randomWeight <= 0 && attackWeights.Values.ElementAt(i) != 0)
            {
                Debug.Log("Selected attack method: " + attackWeights.Keys.ElementAt(i));
                return i+1;
            }
        }

        return 0;
    }
    // Trigger grow animation when player is near
    public override void Attack()
    {   
        if(!isCooledDown || isAttacking) 
            return;

        // Randomly choose an attack method
        switch (GetAttackMethod())
        {
            case 1:
                isAttacking = true;
                isCooledDown = false;
                StartCoroutine(SwipeAttack());
                break;
            case 2:
                isAttacking = true;
                isCooledDown = false;
                StartCoroutine(CastAttack());
                break;
            case 3:
                isAttacking = true;
                isCooledDown = false;
                StartCoroutine(JumpAttack());
                break;
            default:
                Debug.Log("No attack method selected");
                break;
        }
    }
    private IEnumerator SwipeAttack()
    {
        Debug.Log("Attack method: Swipe. Started");
        // Play swipe animation
        // randomly choose attack animation and trigger it
        int swipeAnimationIndex = Random.Range(1, 3);
        _anim.SetTrigger("attack" + swipeAnimationIndex);

        switch (swipeAnimationIndex)
        {
            case 1:
                yield return new WaitForSeconds(0.6f);
                break;
            case 2:
                yield return new WaitForSeconds(1.5f);
                break;
        }

        ApplySwipeDamage();

        switch (swipeAnimationIndex)
        {
            case 1:
                yield return new WaitForSeconds(0.6f);
                break;
            case 2:
                yield return new WaitForSeconds(1f);
                break;
        }
        isAttacking = false;
        StartCoroutine(Cooldown());
    }
    bool canJumpAttack = false;
    private IEnumerator JumpAttack() {
        Debug.Log("Attack method: Jump Attack. Started");

        // Start jump animation
        _anim.SetTrigger("jump");
        Vector3 targetPosition = playerPosition - transform.forward * (attackRange/2);

        while (DistanceIgnoreY(transform.position, targetPosition) > 0.5f)
        {
            // Move towards the player
            GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(transform.position, targetPosition, jumpAttackSpeed * Time.deltaTime));
            
            yield return null;
        }
        Debug.Log("Jump attack landed");
        _anim.SetTrigger("attack1");

        // Apply damage to player
        ApplySwipeDamage(jumpDamage);

        // Wait for jump animation to finish
        yield return new WaitForSeconds(0.6f);

        isAttacking = false;
        StartCoroutine(Cooldown());
    }
    
    private IEnumerator CastAttack() {
        Debug.Log("Attack method: Cast. Started");
        // Start cast animation
        GetComponentInChildren<Animator>().SetTrigger("castStart");

        float adjustmentAngle = Random.Range(-20, 0);
        Vector3 targetPosition = playerPosition - transform.up * 0.5f;
        Vector3 directionToPlayer = playerPosition - targetPosition;
        // Ajust direction to point 45 degrees up from direction to player
        directionToPlayer = Quaternion.Euler(adjustmentAngle, 0, 0) * Quaternion.Euler(87,0,0) * directionToPlayer;

        GameObject castInstance = Instantiate(castProjectile, castPoint.position, Quaternion.identity);
        castInstance.GetComponent<CastProjectile>().targetPosition = playerPosition;
        castInstance.GetComponent<CastProjectile>().startDirection = directionToPlayer;
        castInstance.GetComponent<CastProjectile>().damage = castDamage;

        // Wait for cast charge time
        yield return new WaitForSeconds(castChargeTime);

        // Play cast animation
        _anim.SetTrigger("castEnd");

        yield return new WaitForSeconds(0.55f);

        castInstance.GetComponent<CastProjectile>().Fire();
        
        isAttacking = false;
        StartCoroutine(Cooldown());
    }
    
    public void ApplySwipeDamage(int thisDamage = 20)
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
        if (enableDefaultAttack) {
            Gizmos.color = Color.red;
            Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
            Gizmos.DrawWireSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);

        if (enableJumpAttack) {
            Gizmos.color = Color.green;
            float jumpMidpoint = (jumpAttackDistanceMax + jumpAttackDistanceMin) / 2;
            Gizmos.DrawWireSphere(transform.position+transform.forward*jumpMidpoint, jumpAttackDistanceMax-jumpMidpoint);
        }
    }
}
