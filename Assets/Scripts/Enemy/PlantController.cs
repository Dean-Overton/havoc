using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// All behaviours for groot plant enemy
public class PlantController : EnemyController
{
    [SerializeField] private int attackSwipeDamage = 20;
    [SerializeField] private float turnAndFaceSpeed = 5f;
    [SerializeField] private float turnAndFaceUpdateTime = 1f;
    [Header("Cast Attack Settings")]
    [SerializeField] private float castAttackDistance = 2f;
    [SerializeField] private float castChargeTime = 2f;
    [SerializeField] private GameObject castProjectile;
    [SerializeField] private Transform castPoint;
    
    private void Start() {
        onStateChanged += StateMonitor;
    }
    public void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // Come alive
            GetComponent<Animator>().SetTrigger("goAlive");

            // if only just entered fight state, cooldown before attacking
            canAttack = false;
            StartCoroutine(Cooldown());

            StartCoroutine(UpdateFacingDirection());
        }
        else if(newState == EnemyState.Patrol)
        {
            // Die
            GetComponent<Animator>().SetTrigger("goDead");

            StopCoroutine(UpdateFacingDirection());
        }
    }
    protected override void Update()
    {
        base.Update();

        CastAttackUpdateLogic();
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
                    GetComponent<Animator>().SetFloat("locomotion", -animationSpeed);

                // Update the direction to face the player
                direction = (playerPosition - transform.position).normalized;
                
                // Calculate the rotation towards the player
                targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the target rotation over time
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnAndFaceSpeed);

                // Wait for the next frame before continuing the loop
                yield return null;
            }
            Debug.Log("Now facing player");
            GetComponent<Animator>().SetFloat("locomotion", 0f);

            yield return new WaitForSeconds(turnAndFaceUpdateTime);
        }
    }
    bool canCastAttack = false;
    public void CastAttackUpdateLogic()
    {
        if(Vector3.Distance(transform.position, playerPosition) < castAttackDistance)
        {
            if(canAttack && enemyState == EnemyState.Fight)
            {
                canCastAttack = true;
                DefaultAttack();
            }
        } else {
            canCastAttack = false;
        }
    }
    // Trigger grow animation when player is near
    public override void DefaultAttack()
    {
        // Randomly choose an attack method
        canAttack = false;
        isAttacking = true;
        
        int attackMethod = Random.Range(1, canCastAttack ? 3 : 2);
        switch (attackMethod)
        {
            case 1:
                StartCoroutine(SwipeAttack());
                break;
            case 2:
                StartCoroutine(CastAttack());
                break;
        }
    }
    private IEnumerator SwipeAttack()
    {
        Debug.Log("Attack method: Swipe. Started");
        // Play swipe animation
        // randomly choose attack animation and trigger it
        int swipeAnimationIndex = Random.Range(1, 3);
        GetComponent<Animator>().SetTrigger("attack" + swipeAnimationIndex);

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
    private IEnumerator CastAttack() {
        Debug.Log("Attack method: Cast. Started");
        // Start cast animation
        GetComponentInChildren<Animator>().SetTrigger("castStart");

        float adjustmentAngle = Random.Range(-20, 0);
        Vector3 directionToPlayer = playerPosition - transform.position;
        // Ajust direction to point 45 degrees up from direction to player
        directionToPlayer = Quaternion.Euler(adjustmentAngle, 0, 0) * Quaternion.Euler(87,0,0) * directionToPlayer;

        GameObject castInstance = Instantiate(castProjectile, castPoint.position, Quaternion.identity);
        castInstance.GetComponent<CastProjectile>().targetPosition = playerPosition;
        castInstance.GetComponent<CastProjectile>().startDirection = directionToPlayer;

        // Wait for cast charge time
        yield return new WaitForSeconds(castChargeTime);

        // Play cast animation
        GetComponent<Animator>().SetTrigger("castEnd");

        yield return new WaitForSeconds(0.55f);

        castInstance.GetComponent<CastProjectile>().Fire();
        
        isAttacking = false;
        StartCoroutine(Cooldown());
    }
    public void ApplySwipeDamage()
    {
        // do phyics collision in front of plant and check if player is hit
        // if player is hit, apply damage

        Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                hitCollider.GetComponent<health_component>().ReduceCurrentHealth(attackSwipeDamage);

                break; // Only hit the player once
            }
        }
    
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
        Gizmos.DrawWireSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);
    }
}
