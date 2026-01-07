using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Numerics;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RagedAttack", story: "[self] at to [time] and [pool] to get fire", category: "Action", id: "95888fe541466757f33c9101a191df27")]
public partial class RagedAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Time;
    [SerializeReference] public BlackboardVariable<GameObject> Pool;
    private Animator anim;
    private bool fired = false;
    private bool fireAttacking = false;
    protected override Status OnStart()
    {
        if (!anim) {
            anim = Self.Value.GetComponentInChildren<Animator>();
        }

        if (Time.Value <= 0) {
            Debug.LogError("Time value must be greater than zero.");
            return Status.Failure;
        }

        anim.SetTrigger("fire");
        fired = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var pooler = Pool.Value.GetComponent<ObjectPooler>();

        if (pooler == null) { 
            Debug.LogError("ObjectPooler component not found on Pool GameObject.");
            return Status.Failure;
        }

        
        if (anim == null || !anim.isActiveAndEnabled) {
            Debug.LogError("Animator component not found or not enabled on Self GameObject.");
            return Status.Failure;
        }

        fireAttacking = anim.GetBool("fire_attacking");

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
/*
        var melee_hash = Animator.StringToHash("melee_attack");
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
        }
*/

        /*
        if (stateInfo.IsName("fire_prepare")) {
            return Status.Running;
        }*/

        if (stateInfo.IsName("fire")) {

            

            //if (!fired && stateInfo.normalizedTime >= Time.Value) {
            if (!fired && stateInfo.normalizedTime >= 0.5f) {
                // Spawn projectile from pool
                UnityEngine.Vector3 direction = (Self.Value.transform.forward).normalized;
                UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.LookRotation(direction);
                GameObject projectile = pooler.SpawnFromPool("fire", Self.Value.transform.position + Self.Value.transform.forward * 1.5f, lookRotation);
                //GameObject projectile = pooler.SpawnFromPool("fire", Self.Value.transform.position + Self.Value.transform.forward * 1.5f, Quaternion.identity);
                if (projectile != null) {
                    // Initialize projectile if needed
                    projectile.SetActive(true);
                }
                fired = true;
            }
        }


        if (fired && !fireAttacking) {
            return Status.Success;
        }


        return Status.Running;
    }

    protected override void OnEnd()
    {
        anim.ResetTrigger("fire");
    }
}

