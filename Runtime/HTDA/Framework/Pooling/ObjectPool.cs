using System;
using System.Collections.Generic;

namespace HTDA.Framework.Pooling
{
    public sealed class ObjectPool<T> : IPool<T>
    {
        private readonly Func<T> _create;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;

        private readonly PoolOptions _options;

        private readonly Stack<T> _inactive;
        private readonly LinkedList<T> _activeList;
        private readonly Dictionary<T, LinkedListNode<T>> _activeNodes;

        private int _countAll;
        private bool _disposed;

        public int CountInactive => _inactive.Count;
        public int CountActive => _activeNodes.Count;
        public int CountAll => _countAll;

        public ObjectPool(
            Func<T> create,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            PoolOptions options = default,
            int initialCapacity = 16,
            IEqualityComparer<T> comparer = null)
        {
            _create = create ?? throw new ArgumentNullException(nameof(create));
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;

            _options = options.Equals(default(PoolOptions)) ? new PoolOptions() : options;

            int cap = Math.Max(1, initialCapacity);
            _inactive = new Stack<T>(cap);
            _activeList = new LinkedList<T>();
            _activeNodes = new Dictionary<T, LinkedListNode<T>>(cap, comparer ?? EqualityComparer<T>.Default);

            if (_options.Prewarm > 0)
                Prewarm(_options.Prewarm);
        }

        public void Prewarm(int count)
        {
            if (_disposed) return;

            for (int i = 0; i < count; i++)
            {
                if (_options.MaxSize > 0 && _countAll >= _options.MaxSize) break;

                var item = _create();
                _countAll++;
                _inactive.Push(item);
            }
        }

        public T Get()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ObjectPool<T>));

            T item;

            if (_inactive.Count > 0)
            {
                item = _inactive.Pop();
            }
            else
            {
                // No inactive -> apply policy
                if (_options.MaxSize > 0 && _countAll >= _options.MaxSize)
                {
                    switch (_options.ExpandPolicy)
                    {
                        case PoolExpandPolicy.Fail:
                            return default;

                        case PoolExpandPolicy.ReuseOldest:
                            if (_activeList.First == null) return default;

                            // Reuse oldest active item, move it to the end (most-recent)
                            var node = _activeList.First;
                            item = node.Value;

                            _activeList.RemoveFirst();
                            _activeList.AddLast(node);

                            _onGet?.Invoke(item);
                            return item;

                        default:
                            // Expand policy but cannot expand due to MaxSize -> fail
                            return default;
                    }
                }

                if (_options.ExpandPolicy == PoolExpandPolicy.Fail)
                    return default;

                // Expand
                item = _create();
                _countAll++;
            }

            _onGet?.Invoke(item);

            // Track active O(1)
            var newNode = _activeList.AddLast(item);
            _activeNodes[item] = newNode;

            return item;
        }

        public void Release(T item)
        {
            if (_disposed) return;
            if (EqualityComparer<T>.Default.Equals(item, default)) return;

            // Remove from active O(1)
            if (_activeNodes.TryGetValue(item, out var node))
            {
                _activeNodes.Remove(item);
                _activeList.Remove(node);
            }

            _onRelease?.Invoke(item);
            _inactive.Push(item);
        }

        public void Clear(bool disposeActive = false)
        {
            if (_disposed) return;

            while (_inactive.Count > 0)
            {
                var item = _inactive.Pop();
                _onDestroy?.Invoke(item);
                _countAll--;
            }

            if (disposeActive)
            {
                // Destroy all active
                var node = _activeList.First;
                while (node != null)
                {
                    var next = node.Next;
                    _onDestroy?.Invoke(node.Value);
                    node = next;
                    _countAll--;
                }

                _activeList.Clear();
                _activeNodes.Clear();
            }
            else
            {
                // Keep active instances alive, just forget tracking (rarely used)
                _activeList.Clear();
                _activeNodes.Clear();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Clear(disposeActive: true);
        }
    }
}