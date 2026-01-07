using UnityEngine;

public class NormalBullet : Bullet
{
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
        //other.GetComponent<Boss>().TakeDamage(damage);
        //보스 데미지주기
        Destroy(gameObject);
    }
}
