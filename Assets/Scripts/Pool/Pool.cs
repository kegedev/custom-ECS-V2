using System.Collections.Generic;
using UnityEngine;

namespace Game.Pool
{
    public struct Pool<T>
    {
        public Stack<T> PoolData;
    }
}

