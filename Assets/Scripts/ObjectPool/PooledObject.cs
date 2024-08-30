using UnityEngine;

namespace DesignPatterns.ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        private ObjectPool pool;
        public ObjectPool Pool { get => pool; set => pool = value; }
        public string objectName;
        public int poolAmount;

        public virtual void InitPrefab() { }

        public virtual void Init() { }

        public void Release()
        {
            pool.ReturnToPool(this);
        }
    }
}