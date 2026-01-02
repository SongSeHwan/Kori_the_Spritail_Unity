using UnityEngine;


[System.Serializable]
public class weapon : MonoBehaviour
{
    public enum Type { Basic, Melee, Range,Bomb };

    public Type weaponType = Type.Basic;

    public int AtkDmg = 1;
    public float itemAtkRange = 2.0f;

    public float chgTime = 0.5f;
    public int chgAckDmg = 5;
    public float chgRange = 4.0f;
    public int ChargeAttackBulletCount = 5;
    public int ChargeAttackBulletAngle = 15;
    public int durMax = 10;
    public int durUseAtk = 1;
    public int curDur = 1;
    public float chargingPercent = 0.0f;
    public bool isBreak = false;
    public bool isCompleteCharge = false;
    public float bombThrowDuration = 2.5f;
    public float bombRadius = 2.5f;

    public bool activeWeapon
    {
        set 
        {
            gameObject.SetActive(value);
        }
    }


    public void Enable(bool Enalbe)
    { 
        gameObject.SetActive(Enalbe);
    }

}
