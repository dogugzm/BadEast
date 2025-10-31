using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ObjectPool<T> : IPool where T : class
    {
        private readonly IFactory<T> _factory;
        private readonly Queue<T> _pool = new();

        public ObjectPool(IFactory<T> factory, int initialSize = 0)
        {
            _factory = factory;
            for (var i = 0; i < initialSize; i++)
            {
                var product = _factory.Create();
                if (product is IProduct<T> pooledObject)
                {
                    pooledObject.Initialize(this);
                }

                if (product is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.gameObject.SetActive(false);
                }

                _pool.Enqueue(product);
            }
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                var product = _pool.Dequeue();
                if (product is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.gameObject.SetActive(true);
                }

                return product;
            }

            var newProduct = _factory.Create();
            if (newProduct is IProduct<T> pooledObject)
            {
                pooledObject.Initialize(this);
            }

            return newProduct;
        }

        public void Return(T product)
        {
            if (product is IProduct<T> pooledObject)
            {
                pooledObject.Decommission();
            }
            else if (product is MonoBehaviour monoBehaviour)
            {
                monoBehaviour.gameObject.SetActive(false);
            }

            _pool.Enqueue(product);
        }

        public Type ProductType => typeof(T);

        object IPool.Get()
        {
            return Get();
        }

        void IPool.Return(object obj)
        {
            if (obj is T typedProduct)
            {
                Return(typedProduct);
            }
        }
    }
}