using System;
using System.Collections;
//using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    protected NavMeshAgent _navMeshAgent;
    protected Animator _anim;

    [HideInInspector]
    public Vector3 playerPosition; // Stores player input
    [SerializeField] private GameObject _player;

    // IDLE AND PATROLLING UNTIL PLAYER IS IN RANGE
    // then INTO FIGHT STATE
    public float _playerSightRange = 10f; // Distance at which the enemy will see the player

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
    [Header("Default Attack Settings")]
    [SerializeField] protected bool enableDefaultAttack = true;

    [Tooltip("The distance at which the enemy will attack the player.")]
    public float attackRange = 2f; // Distance at which the enemy will attack the player
    [Tooltip("For offsetting the attack sphere.")]
    public Vector2 attackRangeOffset = new Vector2(0, 0); // Offset for the attack range
    public float attackCooldown = 2f; // Time between attacks

    private void Awake() {
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Start()
    {
        _player = GameObject.Find("Player");

        playerPosition = _player.transform.position;
        enemyState = EnemyState.Patrol;

        canDefaultAttack = false;
        isAttacking = false;
        isCooledDown = true;
    }
    protected virtual void Update()
    {
        playerPosition = _player.transform.position;
        PlayerAttackLogic();
    }
    private void PlayerAttackLogic()
    {
        // Check if the player is within attack range
        if (DistanceIgnoreY(transform.position, playerPosition) < _playerSightRange)
        {
            if (enemyState == EnemyState.Patrol)
            {
                Debug.Log(gameObject.name + " has seen the player and is now in fight mode.");

                // Change the enemy state to fight
                enemyState = EnemyState.Fight;
            }
        }
        if (enemyState == EnemyState.Fight && enableDefaultAttack)
            DefaultAttackUpdateLogic();
    }
    [SerializeField]
    protected bool isAttacking = false;
    [SerializeField]
    protected bool isCooledDown = true;
    protected bool canDefaultAttack = false;
    private void DefaultAttackUpdateLogic()
    {
        // Distance check
        if(DistanceIgnoreY(transform.position, playerPosition) > attackRange)
        {   
            canDefaultAttack = false;
            return;
        }

        // Collider check
        Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);
        colliders = Array.FindAll(colliders, c => c.gameObject.tag == "Player");
        if (colliders.Length == 0) {
            canDefaultAttack = false;
            return;
        }
        
        canDefaultAttack = true;
        Attack();
    }
    protected IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isCooledDown = true;
    }
    public virtual void Attack()
    {
        // Attack the player
        Debug.LogError("The enemy is attacking the player. But the Attack() method is not implemented.");
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _playerSightRange);

        if (enableDefaultAttack) {
            Gizmos.color = Color.red;
            Vector3 offset = new Vector3(attackRangeOffset.x, attackRangeOffset.y, 0);
            Gizmos.DrawWireSphere(transform.position + offset + transform.forward * (attackRange/2), attackRange/2);
        }
    }

    protected Vector2 flattenVector(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }
    protected float DistanceIgnoreY(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(flattenVector(a), flattenVector(b));
    }
}
[System.Serializable]
public enum EnemyState
{
    Patrol, // Enemy is patrolling
    Fight, // Enemy is fighting
    Dead // Enemy is dead
}