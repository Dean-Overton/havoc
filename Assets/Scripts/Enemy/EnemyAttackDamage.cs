using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDamage : StateMachineBehaviour
{
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private float _damageDelay = 1f;
    private float _timer;
    private bool _attacked = false;

    // health_component healthComponent;

    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0f;
        _attacked = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer += Time.deltaTime;
        if (_timer >= _damageDelay && !_attacked)
        {
            DealDamage();
            _timer = 0f;
            _attacked = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0f;
        _attacked = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private void DealDamage()
    {
        health_component healthComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<health_component>();
        if (healthComponent != null)
        {
            healthComponent.ReduceCurrentHealth(_damageAmount);
            Debug.Log($"Applying {_damageAmount} damage");
        }
    }
}
