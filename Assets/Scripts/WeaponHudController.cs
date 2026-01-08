using UnityEngine;
using UnityEngine.InputSystem;

public sealed class WeaponHudController : MonoBehaviour
{
    [Header("Bind Target (Pattern B)")]
    [SerializeField] private int targetPlayerIndex = 0; // 0=1P, 1=2P
    [SerializeField] private float retryInterval = 0.25f;
    [SerializeField] private float timeoutSeconds = 10f;

    [Header("Slot Views (0~3)")]
    [SerializeField] private WeaponSlotItemView[] slotViews = new WeaponSlotItemView[4];

    [Header("Icon Sets")]
    [SerializeField] private WeaponIconSet defaultSet;
    [SerializeField] private WeaponIconSet meleeSet;
    [SerializeField] private WeaponIconSet rangedSet;
    [SerializeField] private WeaponIconSet bombSet;

    [Header("Weapon Source (optional override)")]
    [Tooltip("비워두면 PlayerInput 아래에서 자동 탐색합니다.")]
    [SerializeField] private Transform weaponRootOverride;

    private PlayerInput _owner;
    private weapon[] _weapons = new weapon[4];

    // “현재 선택 슬롯”은 프로젝트마다 저장 위치가 다릅니다.
    // 당장 동작을 위해 외부에서 SetSelectedSlot을 호출하는 방식으로 둡니다.
    private int _selectedSlotIndex = 0;

    private Coroutine _bindCo;

    private void OnEnable()
    {
        // 슬롯 인덱스 텍스트 세팅(WeaponSlotItemView 내부에서 텍스트가 설정됨)
        for (int i = 0; i < slotViews.Length; i++)
        {
            if (slotViews[i] != null)
                slotViews[i].SetSlotIndex(i);
        }

        _bindCo = StartCoroutine(BindOwnerWhenReady());
    }

    private void OnDisable()
    {
        if (_bindCo != null) StopCoroutine(_bindCo);
        _bindCo = null;

        UnsubscribeWeaponEvents();
        _owner = null;
    }

    /// <summary>
    /// 외부(플레이어 컨트롤러/무기 매니저)가 선택 슬롯을 변경할 때 호출.
    /// </summary>
    public void SetSelectedSlot(int slotIndex)
    {
        _selectedSlotIndex = Mathf.Clamp(slotIndex, 0, 3);
        RefreshAll();
    }

    private System.Collections.IEnumerator BindOwnerWhenReady()
    {
        float elapsed = 0f;

        if (TryBindOwnerNow())
            yield break;

        while (elapsed < timeoutSeconds)
        {
            yield return new WaitForSecondsRealtime(retryInterval);
            elapsed += retryInterval;

            if (TryBindOwnerNow())
                yield break;
        }

        Debug.LogWarning($"[WeaponHudController] Failed to bind PlayerInput (playerIndex={targetPlayerIndex}) within timeout.");
    }

    private bool TryBindOwnerNow()
    {
        if (_owner != null)
            return true;

        var inputs = FindObjectsOfType<PlayerInput>(includeInactive: false);
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] != null && inputs[i].playerIndex == targetPlayerIndex)
            {
                BindOwner(inputs[i]);
                return true;
            }
        }

        return false;
    }

    private void BindOwner(PlayerInput owner)
    {
        _owner = owner;
        if (_owner == null)
        {
            Debug.LogError("[WeaponHudController] BindOwner called with null.");
            return;
        }

        // 무기 루트 결정
        var root = weaponRootOverride != null ? weaponRootOverride : _owner.transform;

        // 기존 구독 해제
        UnsubscribeWeaponEvents();

        // 무기 4슬롯 매핑 시도
        // 권장: 인스펙터에서 확정 매핑(아래 AutoMapWeapons는 fallback 용도)
        AutoMapWeapons(root);

        // 무기 변경 이벤트 구독(weapon 클래스에 OnHudChanged가 추가된 상태 가정)
        SubscribeWeaponEvents();

        // 최초 UI 갱신
        RefreshAll();
    }

    /// <summary>
    /// fallback 자동 매핑:
    /// - root 아래 weapon들을 전부 수집
    /// - Type 기준으로 Basic/Melee/Range/Bomb를 각 슬롯에 1개씩 할당
    /// 주의: 동일 Type 무기가 여러 개면 첫 번째만 잡힙니다.
    /// </summary>
    private void AutoMapWeapons(Transform root)
    {
        for (int i = 0; i < _weapons.Length; i++)
            _weapons[i] = null;

        var found = root.GetComponentsInChildren<weapon>(includeInactive: true);

        // 기본 정책(프로젝트 룰에 맞게 조정 가능):
        // slot0=Basic, slot1=Melee, slot2=Range, slot3=Bomb
        AssignFirstOfType(found, weapon.Type.Basic, 0);
        AssignFirstOfType(found, weapon.Type.Melee, 1);
        AssignFirstOfType(found, weapon.Type.Range, 2);
        AssignFirstOfType(found, weapon.Type.Bomb, 3);
    }

    private void AssignFirstOfType(weapon[] found, weapon.Type type, int slot)
    {
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null && found[i].weaponType == type)
            {
                _weapons[slot] = found[i];
                return;
            }
        }
    }

    private void SubscribeWeaponEvents()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
                _weapons[i].OnHudChanged += HandleWeaponHudChanged;
        }
    }

    private void UnsubscribeWeaponEvents()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null)
                _weapons[i].OnHudChanged -= HandleWeaponHudChanged;
        }
    }

    private void HandleWeaponHudChanged(weapon w)
    {
        // 어떤 무기가 바뀌었든 전체 갱신해도 4슬롯이라 부담이 거의 없습니다.
        RefreshAll();
    }

    private void Update()
    {
        // 이벤트가 잘 안 걸리는 상황(구형 코드 경로, 외부에서 curDur 직접 변경 등) 대비:
        // 필요하면 폴링을 켤 수 있습니다. 기본은 꺼두려면 아래 라인 주석 처리.
        // if (_owner != null) RefreshAll();
    }

    private void RefreshAll()
    {
        for (int slot = 0; slot < 4; slot++)
        {
            var view = slotViews[slot];
            if (view == null) continue;

            var w = _weapons[slot];

            bool hasWeapon = (w != null) && w.gameObject.activeInHierarchy && !w.isBreak; // 보유/파손 정책은 프로젝트 룰에 맞게 조정
            bool selected = (slot == _selectedSlotIndex);

            float durability01 = 0f;
            WeaponIconSet set = defaultSet;

            if (w != null)
            {
                durability01 = w.HudDurability01; // weapon에 추가한 HUD 프로퍼티 사용
                set = GetIconSet(w.HudSlotKind);
            }

            view.ApplySnapshot(hasWeapon, selected, durability01, set);
        }
    }

    private WeaponIconSet GetIconSet(WeaponSlotKind kind)
    {
        return kind switch
        {
            WeaponSlotKind.Default => defaultSet != null ? defaultSet : meleeSet,
            WeaponSlotKind.Melee => meleeSet != null ? meleeSet : defaultSet,
            WeaponSlotKind.Ranged => rangedSet != null ? rangedSet : defaultSet,
            WeaponSlotKind.Bomb => bombSet != null ? bombSet : defaultSet,
            _ => defaultSet
        };
    }
}
