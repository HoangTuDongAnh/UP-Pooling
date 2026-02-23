using UnityEngine;
using HTDA.Framework.Pooling;

namespace HTDA.Framework.Pooling.Unity
{
    public sealed class GameObjectPool
    {
        public GameObject Prefab { get; }
        public Transform Root { get; }

        public UnityPoolBehaviour Behaviour { get; }

        private readonly ObjectPool<GameObject> _pool;

        public int CountInactive => _pool.CountInactive;
        public int CountActive => _pool.CountActive;
        public int CountAll => _pool.CountAll;

        public GameObjectPool(GameObject prefab, Transform root = null, PoolOptions options = default, UnityPoolBehaviour behaviour = default)
        {
            Prefab = prefab;
            Root = root;
            Behaviour = behaviour.Equals(default(UnityPoolBehaviour)) ? UnityPoolBehaviour.Default : behaviour;

            _pool = new ObjectPool<GameObject>(
                create: CreateInstance,
                onGet: OnGet,
                onRelease: OnRelease,
                onDestroy: OnDestroy,
                options: options);
        }

        private GameObject CreateInstance()
        {
            var go = Object.Instantiate(Prefab, Root);
            go.name = $"{Prefab.name} (Pooled)";
            go.SetActive(false);

            var tag = go.GetComponent<PooledObject>();
            if (tag == null) tag = go.AddComponent<PooledObject>();

            tag.Owner = this;
            tag.CachePoolables();

            return go;
        }

        private void CallOnSpawned(GameObject go)
        {
            var tag = go.GetComponent<PooledObject>();
            var arr = tag != null ? tag.Poolables : null;
            if (arr == null) return;

            for (int i = 0; i < arr.Length; i++)
                arr[i]?.OnSpawned();
        }

        private void CallOnDespawned(GameObject go)
        {
            var tag = go.GetComponent<PooledObject>();
            var arr = tag != null ? tag.Poolables : null;
            if (arr == null) return;

            for (int i = 0; i < arr.Length; i++)
                arr[i]?.OnDespawned();
        }

        private void CallResetState(GameObject go)
        {
            var tag = go.GetComponent<PooledObject>();
            var arr = tag != null ? tag.Poolables : null;
            if (arr == null) return;

            for (int i = 0; i < arr.Length; i++)
                arr[i]?.ResetState();
        }

        private void OnGet(GameObject go)
        {
            // Spawn sequence:
            // A) Reset -> Activate -> OnSpawned (default)
            // B) Activate -> Reset -> OnSpawned
            if (Behaviour.ResetBeforeActivate)
                CallResetState(go);

            go.SetActive(true);

            if (!Behaviour.ResetBeforeActivate)
                CallResetState(go);

            CallOnSpawned(go);
        }

        private void OnRelease(GameObject go)
        {
            // Despawn sequence:
            // A) OnDespawned -> Deactivate (default)
            // B) Deactivate -> OnDespawned
            if (Behaviour.DespawnCallbacksBeforeDeactivate)
                CallOnDespawned(go);

            // Return under root
            if (Root != null)
                go.transform.SetParent(Root, worldPositionStays: false);

            go.SetActive(false);

            if (!Behaviour.DespawnCallbacksBeforeDeactivate)
                CallOnDespawned(go);
        }

        private static void OnDestroy(GameObject go)
        {
            if (go != null) Object.Destroy(go);
        }

        // ----------- Spawn Overloads -----------

        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null, bool? worldPositionStays = null)
        {
            var go = _pool.Get();
            if (go == null) return null;

            var t = go.transform;

            if (parent != null)
                t.SetParent(parent, worldPositionStays ?? Behaviour.WorldPositionStaysOnSpawnParent);

            t.SetPositionAndRotation(position, rotation);
            return go;
        }

        /// <summary>
        /// Spawn under parent. By default keepLocalTransform=true => localPosition=0, localRotation=identity, localScale=1.
        /// </summary>
        public GameObject Spawn(Transform parent, bool keepLocalTransform = true)
        {
            var go = _pool.Get();
            if (go == null) return null;

            var t = go.transform;

            if (parent != null)
                t.SetParent(parent, worldPositionStays: Behaviour.WorldPositionStaysOnSpawnParent);

            if (keepLocalTransform)
            {
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
            }

            return go;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent = null, bool? worldPositionStays = null) where T : Component
        {
            var go = Spawn(position, rotation, parent, worldPositionStays);
            return go != null ? go.GetComponent<T>() : null;
        }

        public T Spawn<T>(Transform parent, bool keepLocalTransform = true) where T : Component
        {
            var go = Spawn(parent, keepLocalTransform);
            return go != null ? go.GetComponent<T>() : null;
        }

        // ----------- Despawn -----------

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            var tag = instance.GetComponent<PooledObject>();
            if (tag != null && tag.Owner != null && tag.Owner != this)
            {
                // Forward to correct owner
                tag.Owner.Despawn(instance);
                return;
            }

            _pool.Release(instance);
        }

        public void Prewarm(int count) => _pool.Prewarm(count);
        public void Clear(bool destroyActive = false) => _pool.Clear(destroyActive);
        public void Dispose() => _pool.Dispose();
    }
}