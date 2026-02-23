namespace HTDA.Framework.Pooling
{
    public readonly struct PoolOptions
    {
        public readonly int Prewarm;
        public readonly int MaxSize; // 0 = unlimited
        public readonly PoolExpandPolicy ExpandPolicy;

        public PoolOptions(int prewarm = 0, int maxSize = 0, PoolExpandPolicy expandPolicy = PoolExpandPolicy.Expand)
        {
            Prewarm = prewarm < 0 ? 0 : prewarm;
            MaxSize = maxSize < 0 ? 0 : maxSize;
            ExpandPolicy = expandPolicy;
        }
    }
}