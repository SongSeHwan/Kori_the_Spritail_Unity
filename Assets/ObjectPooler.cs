using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
        //poolDictionary 

        foreach (Pool pool in pools) {

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {

                GameObject obj = CreateNewObject(pool.tag, pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject CreateNewObject(string tag, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        return obj;
    }


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn;

        if (poolDictionary[tag].Count == 0) 
        {
            Debug.Log("Pool with tag " + tag + " is empty. Creating new object.");
            Pool pool = pools.Find(p => p.tag == tag);
            objectToSpawn = CreateNewObject(tag, pool.prefab);
        }
        else 
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }

       
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            Destroy(objectToReturn);
            return;
        }
        objectToReturn.SetActive(false);

        poolDictionary[tag].Enqueue(objectToReturn);
    }

}
