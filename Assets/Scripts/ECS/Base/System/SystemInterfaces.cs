using UnityEngine;


namespace Game.ECS.Base.Systems
{
    internal interface ISystem { }
    internal interface IInitSystem : ISystem
    {
        internal void Init(SystemManager systemManager);
    }

    internal interface IUpdateSystem : ISystem
    {
        internal void Update(SystemManager systemManager);
    }

    internal interface IDisposeSystem : ISystem
    {
        internal void Dispose(SystemManager systemManager);
    }


    internal interface ISharedData
    {

    }
}

