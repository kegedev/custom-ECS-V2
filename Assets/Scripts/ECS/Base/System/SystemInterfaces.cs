using UnityEngine;


namespace Game.ECS.Base.Systems
{
    public interface ISystem { }
    public interface IInitSystem : ISystem
    {
        public void Init(SystemManager systemManager);
    }

    public interface IUpdateSystem : ISystem
    {
        public void Update(SystemManager systemManager);
    }

    public interface IDisposeSystem : ISystem
    {
        public void Dispose(SystemManager systemManager);
    }


    public interface ISharedData
    {

    }
}

