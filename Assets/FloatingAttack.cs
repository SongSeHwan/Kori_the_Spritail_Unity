using UnityEngine;

public class FloatingAttack : MonoBehaviour
{
    public float radius = 5f;
    public int damage = 20;
    public float lifetime = 5f; // seconds
    private ObjectPooler objectPooler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        //pooledObject = GetComponent<PooledObject>();
        objectPooler = ObjectPooler.Instance;
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        lifetime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime <= 0) {
            DetactObjectExplosion();
            Debug.Log("boom!!");
            

         
            // Return to pool
            objectPooler.ReturnToPool("floating", this.gameObject);
        }
        else {
            lifetime -= 1 * Time.deltaTime;
        }
    }

    void DetactObjectExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Player")) {
                /*PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null) {
                    playerHealth.TakeDamage(damage);
                }*/
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 기즈모 색상을 빨간색으로 설정
        // 이 스크립트가 붙은 오브젝트의 위치에 explosionRadius 크기의 와이어 구체를 그립니다.
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
