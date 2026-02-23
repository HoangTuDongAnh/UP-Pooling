using UnityEngine;

namespace HTDA.Framework.Pooling.Unity
{
    /// <summary>
    /// Optional marker to limit scanning for IPoolable components to a subtree.
    /// Attach this to a child transform to reduce scan cost for large prefabs.
    /// </summary>
    public sealed class PoolableRoot : MonoBehaviour
    {
    }
}