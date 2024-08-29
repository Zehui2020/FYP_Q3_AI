using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.ObjectPool
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        // PooledObject prefab
        [SerializeField] private PooledObject[] objectsToPool;

        // store the pooled objects in list
        private List<PooledObject> pooledObjects;

        private void Start()
        {
            Instance = this;
            SetupPool();
        }

        // creates the pool (invoke when the lag is not noticeable)
        private void SetupPool()
        {
            // missing objectToPool Prefab field
            if (objectsToPool == null)
            {
                return;
            }

            pooledObjects = new List<PooledObject>();

            // populate the pool
            PooledObject instance = null;

            foreach (PooledObject pooledObject in objectsToPool)
            {
                for (int i = 0; i < pooledObject.poolAmount; i++)
                {
                    instance = Instantiate(pooledObject);
                    instance.InitPrefab();
                    instance.Init();
                    instance.Pool = this;
                    instance.gameObject.SetActive(false);
                    pooledObjects.Add(instance);
                }
            }
        }

        // returns the first active GameObject from the pool
        public PooledObject GetPooledObject(string objectName, bool activateObj)
        {
            // missing objectToPool field
            if (objectsToPool == null)
            {
                return null;
            }

            // if the pool is not large enough, instantiate extra PooledObjects
            if (!CheckObjectPoolCount(objectName))
            {
                foreach (PooledObject pooledObject in objectsToPool)
                {
                    if (pooledObject.objectName == objectName)
                    {
                        PooledObject newInstance = Instantiate(pooledObject);
                        newInstance.Init();
                        newInstance.Pool = this;
                        return newInstance;
                    }
                }
            }

            // otherwise, just grab the next one from the list
            foreach (PooledObject pooledObject in pooledObjects)
            {
                if (pooledObject.objectName == objectName && !pooledObject.gameObject.activeInHierarchy)
                {
                    if (activateObj)
                        pooledObject.gameObject.SetActive(true);

                    return pooledObject;
                }
            }

            return null;
        }

        private bool CheckObjectPoolCount(string objectName)
        {
            foreach (PooledObject pooledObject in pooledObjects)
            {
                if (pooledObject.objectName == objectName && !pooledObject.gameObject.activeInHierarchy)
                    return true;
            }

            return false;
        }

        public void ReturnToPool(PooledObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }
    }
}