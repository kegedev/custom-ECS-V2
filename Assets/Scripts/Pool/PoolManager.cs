using System;
using System.Collections.Generic;


namespace Game.Pool
{
    public class PoolManager
    {
        public Dictionary<Type, object> pools;

        public Pool<T> GetPool<T>()
        {
            if (pools == null) pools = new Dictionary<Type, object>();
            if (pools.TryGetValue(typeof(T), out object val))
            {

                return (Pool<T>)val;
            }
            else return AddPool<T>();
        }

        private Pool<T> AddPool<T>()
        {
            Pool<T> newPool = new Pool<T>() { PoolData = new Stack<T>() };
            pools.Add(typeof(T), newPool);
            return newPool;
        }

    }
}

