using UnityEngine;

public class MeleeAttack1 : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("MeleeAttack1", false);
        int upperBodyLayer = animator.GetLayerIndex("Upper");
        animator.SetLayerWeight(upperBodyLayer, 0f);

        player player = animator.GetComponent<player>();
        player.isAttacking = true;
        player.SetBitAttack();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player player = animator.GetComponent<player>();
        player.SetBitAttack();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player player = animator.GetComponent<player>();
        player.isAttacking = false;
        player.comboElasepdTime = 0f;
        player.SetBitIdle();
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
}
