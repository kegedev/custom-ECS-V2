using Game.Pool;
using System;
using UnityEngine;


namespace Game.Factory.Base
{
    public abstract class BaseFactory<T>
    {
        protected abstract T Create(params object[] arguments);
        protected abstract T Update(ref T component, params object[] arguments);
        public T GetPoolable( Pool<T> pool, params object[] arguments)
        {
            if (pool.PoolData.Count > 0)
            {
                T newT = pool.PoolData.Pop();

                return Update(ref newT, arguments);
            }
            else
            {
                T newPoolable = Create(arguments);
                pool.PoolData.Push(newPoolable);

                return pool.PoolData.Pop();
            }
        }


        //public void ReleasePoolable(Pool<T> pool, IComponent poolable)
        //{
        //    pool.PoolData.Push((T)poolable);
        //}
    }

}

