using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FloatingAttack", story: "[self] around set boom of [pool]", category: "Action", id: "4406753a769fac2c4999dc321cd337de")]
public partial class FloatingAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Pool;
    private bool hasSpawned = false;
    private float spawnTime = 0.5f;
    private float waitTime = 0.5f;
    private float timer = 0f;
    protected override Status OnStart()
    {
        hasSpawned = false;
        timer = 0f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var pooler = Pool.Value.GetComponent<ObjectPooler>();
        if (pooler == null) {
            Debug.LogError("ObjectPooler component not found on Pool GameObject.");
            return Status.Failure;
        }

        Vector3 spawnPosition = Self.Value.transform.position;
        spawnPosition.x += Random.Range(-10f, 10f);
        spawnPosition.z += Random.Range(-10f, 10f);

        if (!hasSpawned) {
            timer += Time.deltaTime;
            if (timer >= spawnTime) {
                GameObject projectile = pooler.SpawnFromPool("floating", spawnPosition, Quaternion.identity);
                hasSpawned = true;
            }
            return Status.Running;
        }
        else {
            timer += Time.deltaTime;
            if (timer >= spawnTime + waitTime) {
                return Status.Success;
            }
            return Status.Running;
        }

        //return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

