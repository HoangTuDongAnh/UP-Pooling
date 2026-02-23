using UnityEngine;
using HTDA.Framework.Pooling;

namespace HTDA.Framework.Pooling.Unity
{
    public sealed class PooledObject : MonoBehaviour
    {
        internal GameObjectPool Owner { get; set; }

        internal IPoolable[] Poolables { get; private set; } = System.Array.Empty<IPoolable>();

        /// <summary>
        /// Cache IPoolable components:
        /// Priority:
        /// 1) ManualPoolables (if present and has entries)
        /// 2) PoolableRoot subtree (if present)
        /// 3) Full prefab subtree (fallback)
        /// </summary>
        internal void CachePoolables()
        {
            // 1) Manual override
            var manual = GetComponent<ManualPoolables>();
            if (manual != null)
            {
                var manualArr = manual.Build();
                if (manualArr != null && manualArr.Length > 0)
                {
                    Poolables = manualArr;
                    return;
                }
            }

            // 2) Limit scan to PoolableRoot subtree
            var root = GetComponentInChildren<PoolableRoot>(true);
            if (root != null)
            {
                Poolables = ScanPoolables(root.transform);
                return;
            }

            // 3) Fallback: scan whole subtree
            Poolables = ScanPoolables(transform);
        }

        private static IPoolable[] ScanPoolables(Transform scanRoot)
        {
            // Scan MonoBehaviours under scanRoot once, filter IPoolable.
            var behaviours = scanRoot.GetComponentsInChildren<MonoBehaviour>(true);

            int count = 0;
            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i] is IPoolable) count++;

            if (count == 0)
                return System.Array.Empty<IPoolable>();

            var arr = new IPoolable[count];
            int idx = 0;
            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i] is IPoolable p) arr[idx++] = p;

            return arr;
        }

        /// <summary>Despawn this instance back to its owner pool.</summary>
        public void Despawn()
        {
            if (Owner != null) Owner.Despawn(gameObject);
            else Destroy(gameObject);
        }
    }
}