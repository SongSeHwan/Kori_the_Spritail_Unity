using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "self patten check", story: "[self] check patten enable", category: "Action", id: "3a0704398e5d300d5d06d237e461f014")]
public partial class SelfPattenCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    Animator animator;

    protected override Status OnStart()
    {
        if (!animator) {
            animator = Self.Value.GetComponentInChildren<Animator>();
        }


        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (animator == null || !animator.isActiveAndEnabled) {
            Debug.LogError("Animator component not found or not enabled on Self GameObject.");
            return Status.Failure;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        var patten_enable_hash = Animator.StringToHash("Idle");

        if (stateInfo.shortNameHash != patten_enable_hash) {
            return Status.Failure;
        }


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

