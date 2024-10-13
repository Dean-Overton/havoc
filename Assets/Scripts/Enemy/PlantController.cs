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

    [Header("Movement")]
    [SerializeField] bool enablePlayerFollow = true;
    
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
            
            if (enablePlayerFollow) {
                StartCoroutine(UpdateTrackingPosition());
            }
        }
        else if(newState == EnemyState.Patrol)
        {
            StopCoroutine(UpdateTrackingPosition());
        }
        else if(newState == EnemyState.Dead) {
            OnDeath ();
        }
    }
    private void OnDeath()
    {
        // Stop all coroutines
        StopAllCoroutines();
        Debug.Log("Plant died");
        // Disable the navmesh agent
        GetComponent<NavMeshAgent>().enabled = false;

        // Disable the collider
        GetComponent<Collider>().enabled = false;

        // Trigger the death animation
        _anim.SetTrigger("death");
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
            
            if (isAttacking || enemyState == EnemyState.Patrol || enemyState == EnemyState.Dead || !enablePlayerFollow)
                _anim.SetFloat("locomotion", 0f);
            
            if (enablePlayerFollow)
                NavMeshFacingUpdate();

                if (_navMeshAgent.isStopped) {
                    if (turnAndFaceCoroutine == null && !CheckFacing(playerPosition)) {
                        turnAndFaceCoroutine = StartCoroutine(TurnAndFace(playerPosition));
                    }
                }
            else {
                if (turnAndFaceCoroutine == null && !CheckFacing(playerPosition)) {
                    turnAndFaceCoroutine = StartCoroutine(TurnAndFace(playerPosition));
                }
            }
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

        isAttacking = true;
        isCooledDown = false;
        // Randomly choose an attack method
        switch (GetAttackMethod())
        {
            case 1:
                StartCoroutine(SwipeAttack());
                break;
            case 2:
                StartCoroutine(CastAttack());
                break;
            case 3:
                StartCoroutine(JumpAttack());
                break;
            default:
                isAttacking = false;
                isCooledDown = true;
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
        while (isAttacking) {
            // Ensure navmesh is set to destination
            if (!_navMeshAgent.hasPath) {
                Vector3 attackDestination = transform.position + transform.forward * jumpAttackDistanceMin;
                _navMeshAgent.SetDestination(attackDestination);
                _navMeshAgent.isStopped = false;
            }
            Debug.Log("Attack method: Jump Attack. Started");

            // Start jump animation
            _anim.SetTrigger("jump");

            float normalSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = jumpAttackSpeed;
            yield return new WaitForSeconds(2f);
            
            Debug.Log("Jump attack landed");
            _anim.SetTrigger("attack1");

            yield return new WaitForSeconds(0.6f);
            // Apply damage to player
            ApplySwipeDamage(jumpDamage);

            // Wait for jump animation to finish
            yield return new WaitForSeconds(0.6f);

            _navMeshAgent.speed = normalSpeed;
            isAttacking = false;
            StartCoroutine(Cooldown());
        }
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
        if (enableJumpAttack) {
            Gizmos.color = Color.green;
            float jumpMidpoint = (jumpAttackDistanceMax + jumpAttackDistanceMin) / 2;
            Gizmos.DrawWireSphere(transform.position+transform.forward*jumpMidpoint, jumpAttackDistanceMax-jumpMidpoint);
        }
    }
}
