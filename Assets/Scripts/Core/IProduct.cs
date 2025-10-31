
namespace Core
{
    public interface IProduct<T> where T : class
    {
        void Initialize(ObjectPool<T> pool);
        void Decommission();
    }
}
