using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BT_InitAction", story: "Init Action set [player1] and [player2] and [pool]", category: "Action", id: "bd079d23f4438f8bc76d5190553500ed")]
public partial class BtInitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player1;
    [SerializeReference] public BlackboardVariable<GameObject> Player2;
    [SerializeReference] public BlackboardVariable<GameObject> Pool;
    protected override Status OnStart()
    {
        GameObject player1 = GameObject.Find("1P");
        GameObject player2 = GameObject.Find("2P");
        GameObject pool = GameObject.Find("ProjectilePooler");
        Player1.Value = player1;
        Player2.Value = player2;
        Pool.Value = pool;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

