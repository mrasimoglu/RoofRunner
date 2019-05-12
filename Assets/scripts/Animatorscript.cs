using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animatorscript : StateMachineBehaviour
{
    public character actor;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actor = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<character>();

        if (stateInfo.IsName("climb_up_right") || stateInfo.IsName("climb_up_left"))
        {
            ladder aLadder = actor.aLadder.GetComponent<ladder>();
            if (actor.aLadder.MoveUp())
            {
                actor.animator.SetTrigger("climb_end");
                animator.SetBool("climbing", false);
                actor.isClimbing = false;
                actor.rigidbody.isKinematic = false;
                actor.rigidbody.MovePosition(aLadder.transform.TransformPoint(aLadder.steps[aLadder.stepCount - 1]));
            }
            else
            {
                actor.rigidbody.MovePosition(aLadder.transform.TransformPoint(aLadder.steps[aLadder.currentStep]));
            }
        }
        else if (stateInfo.IsName("climb_down_right") || stateInfo.IsName("climb_down_left"))
            if (actor.aLadder.MoveDown())
            {
                actor.animator.SetTrigger("climb_end");
                animator.SetBool("climbing", false);
                actor.isClimbing = false;
                actor.rigidbody.isKinematic = false;
            }
            else;
        if (stateInfo.IsName("idle_jump") || stateInfo.IsName("walk_jump") || stateInfo.IsName("run_jump"))
            animator.ResetTrigger("landing");
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


  
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actor = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<character>();

        if (stateInfo.IsName("pulldown"))
        {
            actor.isHolding = false;
            actor.rigidbody.isKinematic = false;
            actor.animator.SetBool("pulldown", false);
        }
        else if (stateInfo.IsName("pullup"))
        {
            actor.isHolding = false;
            actor.rigidbody.isKinematic = false;
            vaultObject aVault = actor.aVaultObject;
            if (aVault)
                actor.rigidbody.MovePosition(aVault.transform.TransformPoint(aVault.curve[1]));
            actor.animator.SetBool("pullup", false);
        }
        else if (stateInfo.IsName("pullup_stair"))
        {
            animator.SetTrigger("climb_end");
            animator.SetBool("pullup", false);
            actor.isClimbing = false;
            actor.rigidbody.isKinematic = false;
        }
        else if (stateInfo.IsName("idle_vault") || stateInfo.IsName("walk_vault") || stateInfo.IsName("run_vault"))
            animator.ResetTrigger("goslide");
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
