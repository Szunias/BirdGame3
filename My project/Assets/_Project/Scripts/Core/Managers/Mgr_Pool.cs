using System.Collections.Generic;
using BirdGame.Core.Interfaces;
using UnityEngine;

namespace BirdGame.Core.Managers
{
    public class Mgr_Pool : MonoBehaviour
    {
        public static Mgr_Pool Instance { get; private set; }

        [System.Serializable]
        public class PoolConfig
        {
            public string poolName;
            public GameObject prefab;
            public int initialSize = 10;
        }

        [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        private Transform _poolParent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _poolParent = new GameObject("PooledObjects").transform;
            _poolParent.SetParent(transform);

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var config in poolConfigs)
            {
                CreatePool(config.poolName, config.prefab, config.initialSize);
            }
        }

        public void CreatePool(string poolName, GameObject prefab, int initialSize)
        {
            if (_pools.ContainsKey(poolName)) return;

            _pools[poolName] = new Queue<GameObject>();
            _prefabs[poolName] = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewInstance(poolName);
            }
        }

        private GameObject CreateNewInstance(string poolName)
        {
            if (!_prefabs.TryGetValue(poolName, out var prefab)) return null;

            var obj = Instantiate(prefab, _poolParent);
            obj.SetActive(false);
            _pools[poolName].Enqueue(obj);
            return obj;
        }

        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(poolName, out var pool))
            {
                Debug.LogError($"Mgr_Pool: Pool '{poolName}' does not exist!");
                return null;
            }

            GameObject obj;
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = CreateNewInstance(poolName);
                if (obj != null) _pools[poolName].Dequeue();
            }

            if (obj == null) return null;

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawn();
            }

            return obj;
        }

        public void Despawn(string poolName, GameObject obj)
        {
            if (!_pools.ContainsKey(poolName))
            {
                Debug.LogError($"Mgr_Pool: Pool '{poolName}' does not exist!");
                Destroy(obj);
                return;
            }

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnDespawn();
            }

            obj.SetActive(false);
            obj.transform.SetParent(_poolParent);
            _pools[poolName].Enqueue(obj);
        }
    }
}
