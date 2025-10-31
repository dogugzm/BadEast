using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class PoolManager
    {
        private readonly IReadOnlyDictionary<Type, IPool> _pools;

        public PoolManager(IEnumerable<IPool> pools)
        {
            _pools = pools.ToDictionary(p => p.ProductType);
        }

        public T Get<T>() where T : class
        {
            if (_pools.TryGetValue(typeof(T), out var pool))
            {
                return pool.Get() as T;
            }

            throw new MissingReferenceException($"Pool for type {typeof(T)} not found.");
        }

        public void Return<T>(T obj) where T : class
        {
            if (_pools.TryGetValue(typeof(T), out var pool))
            {
                pool.Return(obj);
            }
        }
    }
}