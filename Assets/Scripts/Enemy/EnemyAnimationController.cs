using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAnimationController : MonoBehaviour
{
    private EnemyController _enemyController;
    [SerializeField] private Animator _animator;

    public SerializableDictionary<AnimationState, string> animationTriggerStates = new SerializableDictionary<AnimationState, string>()
    {
        {AnimationState.Idle, "isIdle"},
        {AnimationState.Walk, "isWalking"},
        {AnimationState.Run, "isRunning"},
        {AnimationState.Attack, "isAttacking"},
        {AnimationState.Die, "isDying"},
        {AnimationState.Hurt, "isHurt"},
        {AnimationState.Jump, "isJumping"},
        {AnimationState.TurnRight, "isTurningRight"},
        {AnimationState.TurnLeft, "isTurningLeft"},
    };
    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
    }
    private void Start() {
        _enemyController.onAnimationStateChange += StateUpdate;
    }
    private void OnDestroy() {
        _enemyController.onAnimationStateChange -= StateUpdate;
    }
    void StateUpdate(AnimationState newState)
    {
        switch (newState)
        {
            case AnimationState.Walk:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Walk]);
                break;
            case AnimationState.Run:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Run]);
                break;
            case AnimationState.Attack:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Attack]);
                break;
            case AnimationState.Die:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Die]);
                break;
            case AnimationState.Hurt:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Hurt]);
                break;
            case AnimationState.Jump:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Jump]);
                break;
            case AnimationState.TurnRight:
                _animator.SetTrigger(animationTriggerStates[AnimationState.TurnRight]);
                break;
            case AnimationState.TurnLeft:
                _animator.SetTrigger(animationTriggerStates[AnimationState.TurnLeft]);
                break;
            default:
                _animator.SetTrigger(animationTriggerStates[AnimationState.Idle]);
                break;
        }
    }
}
[System.Serializable]
public enum AnimationState
{
    Idle,
    Walk,
    Run,
    Jump,
    Attack,
    Hurt,
    Die,
    TurnRight,
    TurnLeft
}