using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// All behaviours for groot plant enemy
public class PlantController : EnemyController
{
    [SerializeField] private int attackSwipeDamage = 20;
    [Header("Cast Attack Settings")]
    [SerializeField] private float castChargeTime = 2f;
    [SerializeField] private GameObject _castProjectile;
    
    private void Start() {
        onStateChanged += StateMonitor;
    }
    public void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // Come alive
            GetComponentInChildren<Animator>().SetTrigger("goAlive");
        }
    }
    
    // Trigger grow animation when player is near
    public override void Attack()
    {
        // Randomly choose an attack method
        int attackMethod = Random.Range(0, 2);
        switch (attackMethod)
        {
            case 0:
                Debug.Log("Attack method: Cast");
                StartCoroutine(CastAttack());
                break;
            case 1:
                Debug.Log("Attack method: Swipe");
                StartCoroutine(SwipeAttack());
                break;
        }
    }
    private IEnumerator SwipeAttack()
    {
        // Play swipe animation
        // randomly choose attack animation and trigger it
        int swipeAnimationIndex = Random.Range(1, 3);
        GetComponentInChildren<Animator>().SetTrigger("attack" + swipeAnimationIndex);

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
    }
    private IEnumerator CastAttack() {
        // Start cast animation
        GetComponentInChildren<Animator>().SetTrigger("castStart");

        // Wait for cast charge time
        yield return new WaitForSeconds(castChargeTime);

        // Play cast animation
        GetComponentInChildren<Animator>().SetTrigger("castEnd");

        isAttacking = false;
        
        StopCoroutine(CastAttack());
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
        
        isAttacking = false;
    
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
        Gizmos.DrawWireSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);
    }
}
