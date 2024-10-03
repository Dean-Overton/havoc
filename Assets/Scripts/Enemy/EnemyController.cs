using System;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public Vector3 playerPosition; // Stores player input
    [SerializeField] private GameObject _player;

    // IDLE AND PATROLLING UNTIL PLAYER IS IN RANGE
    // then INTO FIGHT STATE

    public event Action<EnemyState> onStateChanged; // Event that triggers when the enemy state changes
    private EnemyState _enemyState = EnemyState.Patrol; // Enemy is patrolling by default
    public EnemyState enemyState  {
        get { return _enemyState; }
        set
        {
            if (value != _enemyState) {
                onStateChanged?.Invoke(value);
            };
            _enemyState = value;
        }
    }

    public event Action<AnimationState> onAnimationStateChange; // Event that triggers when the animation state changes
    private AnimationState _animationState = AnimationState.Idle; // Enemy is idle
    public AnimationState animationState
    {
        get { return _animationState; }
        set
        {
            _animationState = value;
            onAnimationStateChange?.Invoke(value);
        }
    }


    [Tooltip("The distance at which the enemy will attack the player.")]
    public float attackRange = 2f; // Distance at which the enemy will attack the player
    [Tooltip("For offsetting the attack sphere.")]
    public Vector2 attackRangeOffset = new Vector2(0, 0); // Offset for the attack range
    public float attackCooldown = 2f; // Time between attacks
    public float _playerSightRange = 10f; // Distance at which the enemy will see the player

    private void Start()
    {
        // Find gamobject with name "Player"
        _player = GameObject.Find("Player");
        playerPosition = _player.transform.position;
    }
    private void Update()
    {
        playerPosition = _player.transform.position;
        PlayerAttackLogic();
    }
    private void PlayerAttackLogic()
    {
        // Check if the player is within attack range
        if (Vector3.Distance(transform.position, playerPosition) < _playerSightRange)
        {
            if (enemyState == EnemyState.Patrol)
            {
                Debug.Log(gameObject.name + " has seen the player and is in fight mode.");

                // Change the enemy state to fight
                enemyState = EnemyState.Fight;
                StartCoroutine(AttackUpdate());
            }
        }
    }
    protected bool isAttacking = false;
    IEnumerator AttackUpdate()
    {
        while (enemyState == EnemyState.Fight)
        {
            // wait until the enemy is not attacking
            yield return new WaitUntil(() => !isAttacking);

            // wait until the player is in range
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerPosition) < attackRange);
            
            // attack the player
            isAttacking = true;
            Attack();

            // wait until is finished attacking
            yield return new WaitUntil(() => !isAttacking);

            // wait for the attack cooldown
            yield return new WaitForSeconds(attackCooldown);
    }
    }
    public virtual void Attack()
    {
        // Attack the player
        Debug.LogError("The enemy is attacking the player. But the Attack() method is not implemented.");
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);
    }
}
[System.Serializable]
public enum EnemyState
{
    Patrol, // Enemy is patrolling
    Fight, // Enemy is fighting
}