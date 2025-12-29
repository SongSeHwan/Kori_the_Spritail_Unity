using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    Vector2 moveInput;
    Animator anim;
    CharacterController cc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += dir * 5f * Time.deltaTime;

        Vector2 raw = moveInput;

        if (raw.magnitude < 0.1f)
            raw = Vector2.zero;

        Vector3 dir = new Vector3(raw.x, 0, raw.y);

        if (raw != Vector2.zero)
        {
            var h = moveInput.x;
            var v = moveInput.y;
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(h, v) * Mathf.Rad2Deg, 0);
            anim.SetBool("OnMove", true);


        }
        else
        {
            anim.SetBool("OnMove", false);
        }

            cc.Move(dir * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Vector2 v = context.ReadValue<Vector2>();
        Debug.Log($"Move »£√‚µ : {v}");
    }

    public void Attack()
    {
        anim.SetTrigger("MeleeAttack");
    }

    public void CatchAndThrow()
    {

    }

    public void Dash()
    {

    }

   
}
