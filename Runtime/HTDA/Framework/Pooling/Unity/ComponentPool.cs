using UnityEngine;
using HTDA.Framework.Pooling;

namespace HTDA.Framework.Pooling.Unity
{
    public sealed class ComponentPool<T> where T : Component
    {
        private readonly GameObjectPool _goPool;

        public ComponentPool(T prefab, Transform root = null, PoolOptions options = default, UnityPoolBehaviour behaviour = default)
        {
            _goPool = new GameObjectPool(prefab.gameObject, root, options, behaviour);
        }

        public T Spawn(Vector3 position, Quaternion rotation, Transform parent = null, bool? worldPositionStays = null)
            => _goPool.Spawn<T>(position, rotation, parent, worldPositionStays);

        public T Spawn(Transform parent, bool keepLocalTransform = true)
            => _goPool.Spawn<T>(parent, keepLocalTransform);

        public void Despawn(T instance)
        {
            if (instance == null) return;
            _goPool.Despawn(instance.gameObject);
        }

        public void Prewarm(int count) => _goPool.Prewarm(count);
        public void Clear(bool destroyActive = false) => _goPool.Clear(destroyActive);
        public void Dispose() => _goPool.Dispose();
    }
}