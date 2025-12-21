using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Scripts.Framework.Pooling
{
    public sealed class PoolService : IPool
    {
        private sealed class Pool
        {
            private readonly GameObject _prefab;
            private readonly Stack<GameObject> _stack;
            private readonly Transform _root;
            
            public Pool(GameObject prefab, Transform root, int initialSize)
            {
                _prefab = prefab;
                _root = root;
                _stack = new Stack<GameObject>(initialSize);

                Prewarm(initialSize);
            }
            
            public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
            {
                GameObject gameObject = _stack.Count > 0 ? _stack.Pop() : Object.Instantiate(_prefab);
                
                Transform transform = gameObject.transform;
                transform.SetParent(parent);
                transform.SetPositionAndRotation(position, rotation);
                
                gameObject.SetActive(true);
                
                IPoolable poolable = gameObject.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnSpawned();
                }
                
                return gameObject;
            }

            public void Despawn(GameObject instance)
            {
                if (instance == null)
                {
                    return;
                }
                
                IPoolable poolable = instance.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnDespawned();
                }
                
                instance.SetActive(false);
                instance.transform.SetParent(_root, false);
                
                _stack.Push(instance);
            }

            private void Prewarm(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject gameObject = Object.Instantiate(_prefab, _root);
                    gameObject.SetActive(false);
                    _stack.Push(gameObject);
                }
            }
        }
        
        private readonly Dictionary<int, Pool> _poolsByPrefabId;
        private readonly Dictionary<int, GameObject> _prefabsByInstanceId;
        private readonly Transform _root;

        public PoolService(Transform root)
        {
            _root = root;
            _poolsByPrefabId = new Dictionary<int, Pool>();
            _prefabsByInstanceId = new Dictionary<int, GameObject>();
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null || count <= 0)
            {
                return;
            }

            GetOrCreatePool(prefab, count);
        }

        public GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, position, rotation, null);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (prefab == null)
            {
                return null;
            }
            
            Pool pool = GetOrCreatePool(prefab, 0);
            GameObject gameObject = pool.Spawn(position, rotation, parent);
            int instanceId = gameObject.GetInstanceID();
            if (!_prefabsByInstanceId.ContainsKey(instanceId))
            {
                _prefabsByInstanceId.Add(instanceId, prefab);
            }
            
            return gameObject;
        }

        public T Spawn<T>(T prefab) where T : Component
        {
            GameObject gameObject = Spawn(prefab.gameObject);
            if (gameObject == null)
            {
                return null;
            }
            
            return gameObject.GetComponent<T>();
        }
        
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            GameObject gameObject = Spawn(prefab.gameObject, position, rotation, parent);
            if (gameObject == null)
            {
                return null;
            }
            
            return gameObject.GetComponent<T>();
        }
        
        public void Despawn(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }
            
            int instanceId = instance.GetInstanceID();
            if (!_prefabsByInstanceId.TryGetValue(instanceId, out GameObject prefab))
            {
                Object.Destroy(instance);
                return;
            }
            
            int prefabId = prefab.GetInstanceID();
            if (!_poolsByPrefabId.TryGetValue(prefabId, out Pool pool))
            {
                Object.Destroy(instance);
                return;
            }
            
            pool.Despawn(instance);
        }

        public void Despawn(Component instance)
        {
            if (instance == null)
            {
                return;
            }
            
            Despawn(instance.gameObject);
        }

        private Pool GetOrCreatePool(GameObject prefab, int count)
        {
            int prefabId = prefab.GetInstanceID();

            Pool pool;
            if (_poolsByPrefabId.TryGetValue(prefabId, out pool))
            {
                return pool;
            }
            Transform group = new GameObject($"{prefab.name}_Pool").transform;
            group.SetParent(_root, false);
            
            pool = new Pool(prefab, group, count);
            _poolsByPrefabId.Add(prefabId, pool);
            
            return pool;
        }
    }
}