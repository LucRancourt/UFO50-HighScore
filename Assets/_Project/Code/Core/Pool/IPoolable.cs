namespace _Project.Code.Core.Pool
{
    public interface IPoolable
    {
        void OnSpawnFromPool();
        void OnReturnToPool();
    }
}