using UnityEngine;
using System;

[System.Serializable]
public class weapon : MonoBehaviour
{
    // =========================================================
    // 1) Identity / Type
    // =========================================================
    public enum Type { Basic, Melee, Range, Bomb };

    [Header("Identity")]
    [Tooltip("Weapon category used for gameplay logic and HUD mapping.")]
    public Type weaponType = Type.Basic;

    // =========================================================
    // 2) Base Attack
    // =========================================================
    [Header("Attack - Base")]
    [Min(0)]
    [Tooltip("Base attack damage.")]
    public int AtkDmg = 1;

    [Min(0f)]
    [Tooltip("Base attack range (units).")]
    public float itemAtkRange = 2.0f;

    // =========================================================
    // 3) Charge Attack
    // =========================================================
    [Header("Attack - Charge")]
    [Min(0f)]
    [Tooltip("Time to reach charged state (seconds).")]
    public float chgTime = 0.5f;

    [Min(0)]
    [Tooltip("Charged attack damage.")]
    public int chgAckDmg = 5;

    [Min(0f)]
    [Tooltip("Charged attack range (units).")]
    public float chgRange = 4.0f;

    [Min(0)]
    [Tooltip("Number of bullets/projectiles fired during charged attack.")]
    public int ChargeAttackBulletCount = 5;

    [Range(0, 180)]
    [Tooltip("Angle spread (degrees) used during charged attack.")]
    public int ChargeAttackBulletAngle = 15;

    // =========================================================
    // 4) Durability
    // =========================================================
    [Header("Durability")]
    [Min(0)]
    [Tooltip("Maximum durability. (Ignored for Basic if you treat it as infinite)")]
    public int durMax = 10;

    [Min(0)]
    [Tooltip("Durability consumed per attack.")]
    public int durUseAtk = 1;

    [Min(0)]
    [Tooltip("Current durability.")]
    public int curDur = 1;

    [Header("Runtime State")]
    [Range(0f, 1f)]
    [Tooltip("Charging progress (0~1).")]
    public float chargingPercent = 0.0f;

    [Tooltip("True if durability is depleted and weapon is broken.")]
    public bool isBreak = false;

    [Tooltip("True if charge is fully completed.")]
    public bool isCompleteCharge = false;

    // =========================================================
    // 5) Bomb Parameters
    // =========================================================
    [Header("Bomb")]
    [Min(0f)]
    [Tooltip("Throw duration (seconds).")]
    public float bombThrowDuration = 2.5f;

    [Min(0f)]
    [Tooltip("Explosion radius (units).")]
    public float bombRadius = 2.5f;

    // =========================================================
    // 6) Activation (existing interface preserved)
    // =========================================================
    [Header("Activation")]
    [Tooltip("Set to enable/disable this weapon GameObject.")]
    public bool hasOwner = false;
    public bool activeWeapon
    {
        set
        {
            gameObject.SetActive(value);
            NotifyHudChanged(); // HUD support (added)
        }
    }

    // =========================================================
    // 7) HUD SUPPORT (added only, no breaking changes)
    // =========================================================
    public event Action<weapon> OnHudChanged;

    /// <summary>HUD에서 쓰기 좋은 슬롯 타입(WeaponIconSet 선택용)</summary>
    public WeaponSlotKind HudSlotKind
    {
        get
        {
            return weaponType switch
            {
                Type.Basic => WeaponSlotKind.Default,
                Type.Melee => WeaponSlotKind.Melee,
                Type.Range => WeaponSlotKind.Ranged,
                Type.Bomb => WeaponSlotKind.Bomb,
                _ => WeaponSlotKind.Default
            };
        }
    }

    /// <summary>HUD 내구도 비율(0~1). Basic은 0 처리.</summary>
    public float HudDurability01
    {
        get
        {
            if (weaponType == Type.Basic) return 0f;
            if (durMax <= 0) return 0f;
            return Mathf.Clamp01((float)curDur / durMax);
        }
    }

    /// <summary>HUD: 무기 파손 여부</summary>
    public bool HudIsBroken => isBreak;

    /// <summary>HUD: 현재 무기가 사용 가능한 상태인지</summary>
    public bool HudIsUsable
    {
        get
        {
            if (!isActiveAndEnabled) return false;
            if (weaponType == Type.Basic) return true;
            return !isBreak && curDur > 0;
        }
    }

    /// <summary>HUD: 차징 비율(0~1)</summary>
    public float HudCharging01 => Mathf.Clamp01(chargingPercent);

    private void NotifyHudChanged()
    {
        OnHudChanged?.Invoke(this);
    }

    // =========================================================
    // 8) Existing Methods (signatures preserved)
    // =========================================================
    public void Init()
    {
        curDur = durMax;
        isBreak = false;      // 기존 룰과 다르면 이 줄만 제거하면 됩니다.
        NotifyHudChanged();
    }

    public void Enable(bool Enalbe)
    {
        gameObject.SetActive(Enalbe);
        NotifyHudChanged();
    }

    public void DecreaseDur()
    {
        if (weaponType == weapon.Type.Basic) return;

        curDur -= durUseAtk;
        if (curDur <= 0)
        {
            curDur = 0;
            isBreak = true;
        }

        NotifyHudChanged();
    }
}
