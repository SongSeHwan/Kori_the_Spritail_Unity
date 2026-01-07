using Unity.AppUI.Core;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector3 moveDir;
     int damage;
    public float lifeTime = 5.0f;
    public float moveSpeed = 1.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 _origin,Vector3 _moveDir,int _damage)
    {
        transform.position = _origin;
       moveDir = _moveDir;
        damage = _damage;
    }
}
