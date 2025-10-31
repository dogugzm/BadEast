using System;

namespace Core
{
    public interface IPool
    {
        object Get();
        void Return(object obj);
        Type ProductType { get; }
    }
}
