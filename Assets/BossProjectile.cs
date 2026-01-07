using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifetime = 5f; // seconds

    private Rigidbody rb;
    private ObjectPooler objectPooler;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        lifetime = 5f;
        Vector3 direction = transform.forward;
        direction.y = 0; // Keep the projectile level on the y-axis
        direction.Normalize();
        Vector3 vel =  direction * speed;
        rb.linearVelocity.Set(vel.x, 0, vel.z);

    }


    // Update is called once per frame
    void Update()
    {
       Debug.Log(rb.linearVelocity);
       Debug.Log("forward: " + transform.forward);
        if (lifetime <= 0) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // Return to pool
            objectPooler.ReturnToPool("fire", this.gameObject);
        }
        else {
            lifetime -= 1 * Time.deltaTime;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            /*PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(damage);
            }*/
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // Return to pool
            objectPooler.ReturnToPool("fire", this.gameObject);
        }
        
    }
}
