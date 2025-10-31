using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core
{
    public class MonoBehaviourFactory<T> : IFactory<T> where T : MonoBehaviour
    {
        private readonly IObjectResolver _resolver;
        private readonly T _prefab;
        private readonly Transform _parent;

        public MonoBehaviourFactory(IObjectResolver resolver, T prefab, Transform parent = null)
        {
            _resolver = resolver;
            _prefab = prefab;
            _parent = parent;
        }

        public T Create()
        {
            return _resolver.Instantiate(_prefab, _parent);
        }
    }
}