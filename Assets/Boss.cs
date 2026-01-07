using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private BehaviorGraphAgent graphAgent;
    private Blackboard blackboard;
    private BlackboardVariable<int> curHP;
    private InputActionMap actionMap;
    void Start()
    {
        graphAgent = GetComponent<BehaviorGraphAgent>();
        actionMap = GetComponent<PlayerInput>().currentActionMap;
        actionMap.AddBinding("Damege", "<Keyboard>/space");
    }

    // Update is called once per frame
    void Update()
    {

        bool check = graphAgent.GetVariable<int>("curHP", out curHP);

        if (!check) {
            Debug.LogError("Failed to get curHP variable from blackboard.");
        }
        else {
            Debug.Log("Successfully got curHP variable from blackboard." + curHP.Value);
        }
        
    }

    public void Damege(int damage)
    {
        curHP.Value -= damage;
        graphAgent.SetVariableValue<int>("curHP", curHP.Value);
        Debug.Log("Boss took damage"+damage+", current HP: " + curHP.Value);
    }

}
