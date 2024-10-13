using System;
using System.Collections;
//using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float turnAndFaceSpeed = 5f;
    [SerializeField] private float followUpdateRate = 2f;
    [Tooltip("This multiplies with attack range to set the stopping distance of the navmesh agent.")]
    [SerializeField] private float attackOffsetMuliplier = 0.8f;

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

        onStateChanged += StateMonitor;
    }
    protected virtual void Start()
    {
        _player = GameObject.Find("Player");

        playerPosition = _player.transform.position;
        enemyState = EnemyState.Patrol;

        canDefaultAttack = false;
        isAttacking = false;
        isCooledDown = true;

        _navMeshAgent.updateRotation = false;

        GetComponent<health_component>().onDeath += OnDeath;
    }
    private void OnDeath()
    {
        StopAllCoroutines();

        enemyState = EnemyState.Dead;
    }
    private void StateMonitor(EnemyState newState)
    {
        if(newState == EnemyState.Fight)
        {
            // if only just entered fight state, cooldown before attacking
            isCooledDown = false;
            StartCoroutine(Cooldown());

            _navMeshAgent.stoppingDistance = attackRange*attackOffsetMuliplier;
        }
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
    protected IEnumerator UpdateTrackingPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isAttacking);

            float distanceToPlayer = DistanceIgnoreY(transform.position, playerPosition);

            if (distanceToPlayer > attackRange*attackOffsetMuliplier) {
                if (_navMeshAgent.destination != playerPosition) {
                    _navMeshAgent.isStopped = false;
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
    protected void NavMeshFacingUpdate() {
        if (_navMeshAgent.hasPath) {            
            Vector3 direction = _navMeshAgent.desiredVelocity.normalized;

            // Optionally, rotate the character to face the direction of movement
            if (direction != Vector3.zero)
            {
                if (turnAndFaceCoroutine == null && !CheckFacing(transform.position + direction)) {
                    turnAndFaceCoroutine = StartCoroutine(TurnAndFace(transform.position + direction));
                }
            }
        }
    }
    private Coroutine _turnAndFaceCoroutine = null;
    protected Coroutine turnAndFaceCoroutine
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
    protected virtual IEnumerator TurnAndFace(Vector3 targetPosition, float turnAndFaceSpeedModifier = 1f)
    {        
        // Calculate direction to the target
        Vector3 directionToTarget = targetPosition - transform.position;

        // if already facing target position, return
        float initialAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        if (Mathf.Abs(initialAngle) < 1f) {
            yield break;
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
        
        // Signal that the coroutine has finished
        turnAndFaceCoroutine = null;
    }
    protected bool CheckFacing (Vector3 targetPosition) {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        return Mathf.Abs(angle) < 1f;
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
    virtual public void DealDamage(int damageAmount)
    {
        // Deal damage to the player
        Debug.LogError("The enemy is dealing damage to the player. But the DealDamage() method is not implemented.");
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