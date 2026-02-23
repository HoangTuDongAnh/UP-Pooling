using UnityEngine;
using HTDA.Framework.Pooling;

namespace HTDA.Framework.Pooling.Unity
{
    /// <summary>
    /// Optional manual list of poolables (zero scan).
    /// If provided, PooledObject will use this list instead of scanning.
    /// </summary>
    public sealed class ManualPoolables : MonoBehaviour
    {
        [Tooltip("Manually assign components that implement IPoolable. If set, pooling will not scan.")]
        [SerializeField] private MonoBehaviour[] poolables;

        public IPoolable[] Build()
        {
            if (poolables == null || poolables.Length == 0)
                return System.Array.Empty<IPoolable>();

            // Convert (filter only IPoolable)
            int count = 0;
            for (int i = 0; i < poolables.Length; i++)
                if (poolables[i] is IPoolable) count++;

            if (count == 0)
                return System.Array.Empty<IPoolable>();

            var arr = new IPoolable[count];
            int idx = 0;
            for (int i = 0; i < poolables.Length; i++)
                if (poolables[i] is IPoolable p) arr[idx++] = p;

            return arr;
        }
    }
}