namespace HTDA.Framework.Pooling
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
        void ResetState();
    }
}