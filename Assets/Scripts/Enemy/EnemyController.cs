using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public Vector3 playerPosition; // Stores player input
    [SerializeField] private GameObject _player;

    // IDLE AND PATROLLING UNTIL PLAYER IS IN RANGE
    // then INTO FIGHT STATE

    public EnemyState enemyState = EnemyState.Patrol; // Enemy is patrolling
    public AnimationState animationState = AnimationState.Idle; // Enemy is idle

    public float attackRange = 2f; // Distance at which the enemy will attack the player
    public float _playerSightRange = 50f; // Distance at which the enemy will see the player

    private void Start()
    {
        // Find gamobject with name "Player"
        _player = GameObject.Find("Player");
        playerPosition = _player.transform.position;
    }
    void Update()
    {
        playerPosition = _player.transform.position;
        PlayerAttackLogic();
    }
    private void PlayerAttackLogic()
    {
        // Check if the player is within attack range
        if (Vector3.Distance(transform.position, playerPosition) < _playerSightRange)
        {
            // Change the enemy state to fight
            enemyState = EnemyState.Fight;
        }
    }
    // public void Attack()
    // {
    //     // Attack the player
    // }
}
[System.Serializable]
public enum AnimationState
{
    Idle,
    Walk,
    Run,
    Jump,
    Attack,
}
[System.Serializable]
public enum EnemyState
{
    Patrol, // Enemy is patrolling
    Fight, // Enemy is fighting
}