using System.Collections.Generic;
using UnityEngine;
using HTDA.Framework.Pooling;

namespace HTDA.Framework.Pooling.Unity
{
    public static class PoolRegistry
    {
        private static readonly Dictionary<int, GameObjectPool> _pools = new Dictionary<int, GameObjectPool>(128);
        private static Transform _root;

        public static void SetRoot(Transform root) => _root = root;

        public static GameObjectPool GetOrCreate(GameObject prefab, PoolOptions options = default, UnityPoolBehaviour behaviour = default)
        {
            if (prefab == null) return null;

            int key = prefab.GetInstanceID();
            if (_pools.TryGetValue(key, out var pool))
                return pool;

            pool = new GameObjectPool(prefab, _root, options, behaviour);
            _pools[key] = pool;
            return pool;
        }

        public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null, PoolOptions options = default, UnityPoolBehaviour behaviour = default)
        {
            var pool = GetOrCreate(prefab, options, behaviour);
            return pool != null ? pool.Spawn(pos, rot, parent) : null;
        }

        public static GameObject Spawn(GameObject prefab, Transform parent, bool keepLocalTransform = true, PoolOptions options = default, UnityPoolBehaviour behaviour = default)
        {
            var pool = GetOrCreate(prefab, options, behaviour);
            return pool != null ? pool.Spawn(parent, keepLocalTransform) : null;
        }

        public static void Despawn(GameObject instance)
        {
            if (instance == null) return;

            var tag = instance.GetComponent<PooledObject>();
            if (tag != null && tag.Owner != null)
            {
                tag.Owner.Despawn(instance);
                return;
            }

            Object.Destroy(instance);
        }

        public static void ClearAll(bool destroyActive = false)
        {
            foreach (var kv in _pools)
                kv.Value.Clear(destroyActive);

            _pools.Clear();
        }
    }
}