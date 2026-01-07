using Unity.AppUI.UI;
using UnityEngine;

public class SpecialBullet : Bullet
{
    public float explosionRaduis = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // 
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Boss")) return;
        Vector3 center = transform.position;
        Collider[] hits = Physics.OverlapSphere(center, explosionRaduis);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Boss"))
            {
                //other.GetComponent<Boss>().TakeDamage(damage);
                //보스 데미지주기
                //폭발이펙트 재생
            }
        }
       
       
        Destroy(gameObject);
    }
}
