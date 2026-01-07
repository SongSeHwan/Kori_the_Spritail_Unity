using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.AppUI.Core;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MeleeAtack",description: "MeleeAtack", story: "[self] at forword MeleeAtack to [Range] is [damage] Atack and [Animation]", category: "Action", id: "20ba3a6db2b808a3a2b2796634fd3c49")]
public partial class MeleeAtackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<int> Damage;
    [SerializeReference] public BlackboardVariable<AnimationClip> Animation;
    private Animator anim;
    private bool attacked = false;
    private bool attacking = false;
    protected override Status OnStart()
    {
        if (!anim) {
            anim = Self.Value.GetComponentInChildren<Animator>();
        }

        if (anim != null) { 
            anim.SetTrigger("melee");
        }

        attacked = false;
        attacking = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 dir = Self.Value.transform.forward;
      
        if(anim == null || !anim.isActiveAndEnabled) {
            Debug.LogError("Animator component not found or not enabled on Self GameObject.");
            return Status.Failure;
        }

        attacking = anim.GetBool("melee_attacking");
        

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);


        
       /* var melee_hash = Animator.StringToHash("melee_attack");
        var melee_prepare_hash = Animator.StringToHash("melee_prepare");
        var fire_hash = Animator.StringToHash("fire");
        var fire_prepare_hash = Animator.StringToHash("fire_prepare");

        switch (stateInfo.shortNameHash) {
            case var hash when hash == melee_hash:
                Debug.Log("In melee_attack state, normalized time: " + stateInfo.normalizedTime);
                break;
            case var hash when hash == melee_prepare_hash:
                Debug.Log("In melee_prepare state, normalized time: " + stateInfo.normalizedTime);
                break;
            case var hash when hash == fire_hash:
                Debug.Log("In fire state, normalized time: " + stateInfo.normalizedTime);
                break;
            case var hash when hash == fire_prepare_hash:
                Debug.Log("In fire_prepare state, normalized time: " + stateInfo.normalizedTime);
                break;
            default:
                Debug.Log("In unknown state: " + stateInfo.shortNameHash);
                break;
        }*/


        /*if (stateInfo.IsName("melee_prepare")) {
            return Status.Running;
        }*/

        if (stateInfo.IsName("melee_attack")) {
            if (stateInfo.normalizedTime >= 0.5f) {
                if (!attacked) {
                    RaycastHit hit;
                    if (Physics.Raycast(Self.Value.transform.position, dir, out hit, Range.Value)) {
                        Debug.Log("Melee Atack Hit: " + hit.collider.name);
                    }
                    attacked = true;
                }
                
            }
        }

        if (attacked&&!attacking) {
            return Status.Success;
        }

        return Status.Running;

    }

    protected override void OnEnd()
    {

    }
}

