using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAnimationController : MonoBehaviour
{
    private EnemyController _enemyController;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
    }
    // Update is called once per frame
    void Update()
    {
        switch (_enemyController.animationState)
        {
            case AnimationState.Walk:
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isRunning", false);
                _animator.SetBool("isAttacking", false);
                break;
            case AnimationState.Run:
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isRunning", true);
                _animator.SetBool("isAttacking", false);
                break;
            case AnimationState.Attack:
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isRunning", false);
                _animator.SetBool("isAttacking", true);
                break;
            default:
                _animator.SetBool("isWalking", false);
                break;
        }
    }
    public void TurnRight()
    {
        _animator.SetTrigger("turnRight");
    }
    public void TurnLeft()
    {
        _animator.SetTrigger("turnLeft");
    }
}
