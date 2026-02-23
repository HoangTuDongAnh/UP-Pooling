using System;

namespace HTDA.Framework.Pooling
{
    public interface IPool<T> : IDisposable
    {
        int CountInactive { get; }
        int CountActive { get; }
        int CountAll { get; }

        T Get();
        void Release(T item);

        void Prewarm(int count);
        void Clear(bool disposeActive = false);
    }
}