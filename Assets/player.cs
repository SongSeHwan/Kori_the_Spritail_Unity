using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GLTFast.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum StateFlag
{
    CanMove = 1<< 0,
    CanAttack = 1 << 1,
    CanThrow = 1 << 2,
    CanSwap = 1 << 3,
    CanDash = 1 << 4,
    CanGrab = 1<< 5
    
}
public class player : MonoBehaviour
{
    public StateFlag state;
    Vector2 moveInput;
    Animator anim;
    CharacterController cc;
    public GameObject handsocket;
    public int MaxHP =100;
    int curHP = 100;
    public int playerIndex = 0;
    public float moveSpeed = 1.0f;
    public int Atk = 1;
    int comboCount = 0;
    float comboDuration = 0.5f;
    public float comboElasepdTime = 0f;
    public bool isAttacking = false;
    float rapidfireTime = 0.5f;
    float rapidfireElapsedTime = 0f;
    bool canRapidfire = false;
    public float rangedAutoAimRange = 10.0f;
	float rangeAngle = 150.0f;
    bool canMeleeCancel = false;
    float chargingTime = 0.0f;
    bool isCharging = false;
    bool isChargeAttack = false;

    float nearDistance = float.MaxValue;

    int countRangeAttack = 0;
    public int countSpecialBullet = 5;
    public float MaxThrowDistance = 2.5f;
    public float bombMoveSpeed = 0.05f;

    public GameObject basicWeaponPrefab;
    public GameObject meleeWeaponPrefab;
    public GameObject rangeWeaponPrefab;
    public GameObject bombWeaponPrefab;
    int weaponIndex = 0;
    List<weapon> weaponInven = new List<weapon>();
    weapon curWeapon;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        curHP = MaxHP;

        SetBitIdle();
       GameObject newweapon = Instantiate
            (
       basicWeaponPrefab,
       handsocket.transform );

        newweapon.transform.localPosition = Vector3.zero;
        newweapon.transform.localRotation = Quaternion.identity;
        weaponInven.Add(newweapon.GetComponent<weapon>());
        curWeapon = newweapon.GetComponent<weapon>();

        GameObject newwweapon = Instantiate
           (
      rangeWeaponPrefab,
      handsocket.transform);
        newwweapon.transform.localPosition = Vector3.zero;
        newwweapon.transform.localRotation = Quaternion.identity;
        weaponInven.Add(newwweapon.GetComponent<weapon>());
        curWeapon = newwweapon.GetComponent<weapon>();


    }

    // Update is called once per frame
    void Update()
    {
        CharMove();
        CheckComboTime();


    }

    private void CharMove()
    {
        if ((state & StateFlag.CanMove) == 0) return;
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

        cc.Move(dir * moveSpeed * Time.deltaTime);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Vector2 v = context.ReadValue<Vector2>();
    }

    private void CheckComboTime()
    {
        if (comboCount != 0)
        {
            comboElasepdTime += Time.deltaTime;
            if (comboElasepdTime >= comboDuration)
            {
                comboCount = 0;
                comboElasepdTime = 0;
                Debug.Log($"콤보초기화됨: ");
            }

        }
    }

    public void Cancancel()
    {
        if (isChargeAttack) return;

        canMeleeCancel = true;
        comboCount = (comboCount + 1) % 3;

        Debug.Log($"cancancel 호출됨: ");
    }

    public void EndAttack()
    {
        
        isAttacking = false;
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (isAttacking) return;
        if (ctx.performed)
        {
            if (curWeapon.weaponType == weapon.Type.Basic || curWeapon.weaponType == weapon.Type.Melee)
            {
                if (isAttacking == false || canMeleeCancel == true)
                {
                    if (comboCount == 0)
                    {
                        anim.SetBool("MeleeAttack1", true);
                        comboElasepdTime = 0;
                    }
                    if (comboCount == 1)
                    {
                        anim.SetBool("MeleeAttack2", true);
                        comboElasepdTime = 0;
                    }
                    if (comboCount == 2)
                    {
                        anim.SetBool("MeleeAttack3", true);
                        comboElasepdTime = 0;
                    }
                    canMeleeCancel = false;
                }
            }
            else if(curWeapon.weaponType == weapon.Type.Range)
            {

                anim.SetBool("RangeAttack1", true);
            }

            Debug.Log($"Attack 호출됨: ");
        }
    }

    public void OnCatchAndThrow()
    {

    }

    public void OnDash()
    {

    }

   
    public void SetBitIdle()
    {
        state = 0;
        state |= StateFlag.CanMove | StateFlag.CanAttack | StateFlag.CanGrab | StateFlag.CanSwap | StateFlag.CanDash;

    }

    public void  SetBitAttack()
    {
        state = 0;
        state |= StateFlag.CanAttack;
    }

    public void SetBitStun()
    {
        state = 0;
    }


    void SwapWeaponInternal(int dir)
    {
        if ((state & StateFlag.CanSwap) == 0) return;
        if (true == isCharging) return;
        int adjustedDirection = dir;
        if (playerIndex == 1)
        {
            adjustedDirection *= -1; // 플레이어2는 방향 반대로
        }

        int maxInventoryIndex = weaponInven.Count -1;
        int maxAllowedIndex = Mathf.Clamp(maxInventoryIndex, 0, 3);
        int slotCount = maxAllowedIndex + 1;
        if (slotCount <= 1) return;
        int prevIndex = weaponIndex;

        int nextIndex = weaponIndex + adjustedDirection;
        nextIndex = ((nextIndex % slotCount) + slotCount) % slotCount;

        if (prevIndex == nextIndex) return;

        weaponIndex = nextIndex;

        if(curWeapon != null)
        {
            curWeapon.activeWeapon = false;
            curWeapon = weaponInven[weaponIndex];
            curWeapon.activeWeapon = true;

            comboCount = 0;
        }

    }


    public void SwapWeaponLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SwapWeaponInternal(-1);
        }
    }

    public void SwapWeaponRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SwapWeaponInternal(1);
        }
    }
}
