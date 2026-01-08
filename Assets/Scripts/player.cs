using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GLTFast.Schema;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum StateFlag
{
    CanMove = 1 << 0,
    CanAttack = 1 << 1,
    CanThrow = 1 << 2,
    CanSwap = 1 << 3,
    CanDash = 1 << 4,
    CanGrab = 1 << 5
    
}
public class player : MonoBehaviour
{
    public StateFlag state;
    Vector2 moveInput;
    Animator anim;
    CharacterController cc;
    public GameObject handsocket;
    public GameObject ShootPos;
    public int MaxHP =100;
    int curHP = 100;
    public int playerIndex = 0;
    public float moveSpeed = 1.0f;
    public int Atk = 1;
    int comboCount = 0;
    public float comboDuration = 0.5f;
    public float comboElasepdTime = 0f;
    public bool isAttacking = false;
    public float rangedAutoAimRange = 10.0f;
	float rangeAngle = 150.0f;
    public bool canMeleeCancel = false;
    bool isCharging = false;

    float nearDistance = float.MaxValue;

    int countRangeAttack = 0;
    public int countSpecialBullet = 5;
    public GameObject basicWeaponPrefab;
    public GameObject meleeWeaponPrefab;
    public GameObject rangeWeaponPrefab;
    public GameObject bombWeaponPrefab;
    public GameObject normalBullet;
    public GameObject specialBullet;
    int weaponIndex = 0;
    List<weapon> weaponInven = new List<weapon>();
    public weapon curWeapon;
    public bool sucessAttack = false;

    public float HitGracePeriodTime = 2.0f;
    public float curGracePeriodTime = 0.0f;
    public  bool OnInvincibility = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        curHP = MaxHP;

        SetBitIdle();
       GameObject newweapon = Instantiate
            (
       basicWeaponPrefab);
        AddWeapon(newweapon.GetComponent<weapon>());
        GameObject rangeweapon = Instantiate
           (
      rangeWeaponPrefab);
        AddWeapon(rangeweapon.GetComponent<weapon>());


    }

    // Update is called once per frame
    void Update()
    {
        CheckComboTime();
        if(curWeapon.isBreak ==true && sucessAttack == true)
        {
            DeleteCurWeapon();
        }

        if(sucessAttack == true)
        {
            sucessAttack = false;
        }


        if(OnInvincibility)
        {
            curGracePeriodTime -= Time.deltaTime;
            if(curGracePeriodTime <= 0.0f)
            {
                curGracePeriodTime = 0.0f;
                OnInvincibility = false;
            }
        }
        if (isDashing)
        {
            DashMove();
            return;
        }
        CharMove();
    }

    public void sendDamage(int _damage)
    {
        if (OnInvincibility) return;
        anim.SetTrigger("OnHit");

        Damage(_damage);
    }

    void Damage(int _damage)
    {
        curHP -= _damage;
        if (curHP <= 0)
        {
            curHP = 0;
        }
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
            }

        }
    }

    public void Cancancel()
    {

        canMeleeCancel = true;
        comboCount = (comboCount + 1) % 3;
    }

    public void EndAttack()
    {
        
        isAttacking = false;
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        Debug.Log("키 처음 눌렀을 때만 실행됨");
        if (isAttacking) return;
        if (curWeapon.isBreak == true) return;
        anim.ResetTrigger("MeleeAttack1");
        anim.ResetTrigger("MeleeAttack2");
        anim.ResetTrigger("MeleeAttack3");
        //if (ctx.started)
        {
            if (curWeapon.weaponType == weapon.Type.Basic || curWeapon.weaponType == weapon.Type.Melee)
            {
                if (isAttacking == false || canMeleeCancel == true)
                {
                    if (comboCount == 0)
                    {
                        anim.SetTrigger("MeleeAttack1");
                        comboElasepdTime = 0;
                    }
                    else if (comboCount == 1)
                    {
                        anim.SetTrigger("MeleeAttack2");
                        comboElasepdTime = 0;
                    }
                    else if (comboCount == 2)
                    {
                        anim.SetTrigger("MeleeAttack3");
                        comboElasepdTime = 0;
                    }
                }
                canMeleeCancel = false;
            }
            else if(curWeapon.weaponType == weapon.Type.Range)
            {

                anim.SetTrigger("RangeAttack");
            }
        }
    }

    public void OnCatchAndThrow()
    {

    }
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 1.0f;
    bool isDashing = false;
    float dashTime = 0f;
    float lastDashTime = -999f;
    Vector3 dashDir;
    public void OnDash()
    {
        if (isDashing) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        isDashing = true;
        dashTime = dashDuration;
        lastDashTime = Time.time;

        dashDir = transform.forward;
        anim.SetTrigger("OnDash");

    }

    void DashMove()
    {
        cc.Move(dashDir * dashSpeed * Time.deltaTime);

        dashTime -= Time.deltaTime;
        if (dashTime <= 0f)
        {
            isDashing = false;
        }
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
            countRangeAttack = 0;
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



    public void ShootBullet()
    {
        if (countRangeAttack != 0 && (countRangeAttack + 1) % countSpecialBullet == 0)
        {
            ShootSpecialBullet();
            countRangeAttack = 0;
        }
        else
        {
            ShootNormalBullet();
            countRangeAttack += 1;
        }
    }

    void ShootSpecialBullet()
    {
        GameObject specialBulletObj = Instantiate(specialBullet);

        SpecialBullet bulletScript = specialBulletObj.GetComponent<SpecialBullet>();
        bulletScript.Init(ShootPos.transform.position,transform.forward, curWeapon.AtkDmg + Atk);
    }

    void ShootNormalBullet()
    {
        GameObject normalBulletObj = Instantiate(normalBullet);
        NormalBullet bulletScript = normalBulletObj.GetComponent<NormalBullet>();
        bulletScript.Init(ShootPos.transform.position,transform.forward,curWeapon.AtkDmg + Atk);
    }


    [SerializeField] LayerMask bossLayer;
    void MeleeAttackOnce(float degAngleRange, float degIntervalAngle)
    {
        bool AttackEnd = false;
        Vector3 rayOrgin = transform.position;
        rayOrgin.y += 0.5f;

        float distacne = curWeapon.itemAtkRange;
        float startAngle = -degAngleRange / 2.0f;
        int rayCount = (int)(degAngleRange / degIntervalAngle) + 1;
        Vector3 forward = transform.forward;
        for (int i  =0; i< rayCount; i++)
        {
            float angle = startAngle + i * degIntervalAngle; 
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * forward;
            dir.Normalize();
            Ray ray = new Ray(rayOrgin, dir);
            RaycastHit hit;
            Debug.DrawRay(rayOrgin, dir * distacne, Color.red, 0.1f);
            if (Physics.Raycast(ray, out hit, curWeapon.itemAtkRange, bossLayer))
            {


                if (!AttackEnd)
                {
                    //보스때리기 
                    AttackEnd = true;
                    break;
                }

            }

        }
       
    }


    [SerializeField] GameObject Slash1Effect;
    [SerializeField] GameObject Slash2Effect;
    [SerializeField] GameObject Slash3Effect;
    public void MeleeAttack1()
    {
        MeleeAttackOnce(180, 15);
        if (comboCount == 0)
        {
            GameObject effectObj = Instantiate(Slash1Effect);
            Vector3 pos = transform.position;
            pos += transform.forward * 1.0f;
            pos.y += 0.5f;
            effectObj.transform.position = pos;
        }
        else
        {
            GameObject effectObj = Instantiate(Slash2Effect);

            Vector3 pos = transform.position;
            pos += transform.forward * 1.0f;
            pos.y += 0.5f;
            effectObj.transform.position = pos;
        }

    }
    public void Meleeattack3()
    {
        MeleeAttackOnce(360, 15);
        GameObject effectObj = Instantiate(Slash3Effect);
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        effectObj.transform.position = pos;

    }


    public void AddWeapon(weapon _weapon)
    {
        weaponInven.Add(_weapon);
        _weapon.gameObject.transform.SetParent(handsocket.transform);
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;
        _weapon.Init();
        _weapon.hasOwner =true;
        if (weaponInven.Count == 1)
        {
            curWeapon = _weapon;
            curWeapon.activeWeapon = true;
            weaponIndex = 0;
        }
        else
        {
            _weapon.activeWeapon = false;
        }
    }

    void DeleteCurWeapon()
    {
        if (curWeapon == null) return;
        if (curWeapon.weaponType == weapon.Type.Basic) return;

        weapon deleteweapon = curWeapon;
        SwapBasicWeapon();
        weaponInven.Remove(deleteweapon);
        Destroy(deleteweapon.gameObject);
     


    }
    void SwapBasicWeapon()
    {
        weaponIndex = 0;
        curWeapon.activeWeapon = false;
        curWeapon = weaponInven[0];
        curWeapon.activeWeapon = true;
        comboCount = 0;
        countRangeAttack = 0;
    }



    GameObject nearObject;
    private void OnTriggerEnter(Collider other)
    {

        if (weaponInven.Count >= 4) return;
        if (other.TryGetComponent<weapon>(out weapon newweapon))
        {
            if (newweapon.hasOwner == false)
            {
                AddWeapon(newweapon);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
