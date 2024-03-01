using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBehavior : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.isTurning = false;
        PlayerMovement.reversedTurn = false;
        animator.ResetTrigger("turn");
        animator.SetBool("exit_turn", false);
    }
}
