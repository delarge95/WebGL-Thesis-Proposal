using System.Collections.Generic;
using UnityEngine;

namespace WebGL.Core.Utils
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public class ObjectPooler : PersistentSingleton<ObjectPooler>
    {
        [Header("Pool Configuration")]
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                // Create a parent object to keep hierarchy clean
                GameObject poolParent = new GameObject($"Pool_{pool.tag}");
                poolParent.transform.SetParent(transform);

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(poolParent.transform);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Optional: Interface for pooled objects to reset themselves
            // IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            // if (pooledObj != null) pooledObj.OnObjectSpawn();

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}
