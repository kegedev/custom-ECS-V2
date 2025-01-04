using System.Collections.Generic;
using UnityEngine;


namespace Game.ECS.Base.Systems
{
    public class SystemManager
    {
        private ECSWorld GameWorld;

        private List<ISystem> allSystems;
        private List<IInitSystem> initSystems;
        private List<IUpdateSystem> updateSystems;
        private List<IDisposeSystem> destroySystems;
        private List<ISharedData> _sharedData;

        private IUpdateSystem[] updateSystemsArray;


        public SystemManager(ECSWorld gameWorld)
        {
            GameWorld = gameWorld;

            initSystems = new List<IInitSystem>();
            updateSystems = new List<IUpdateSystem>();
            destroySystems = new List<IDisposeSystem>();
            _sharedData = new List<ISharedData>();
        }

        public void AddSystem(ISystem system)
        {
            if (system is IInitSystem initSystem)
            {
                initSystems.Add(initSystem);
            }

            if (system is IUpdateSystem updateSystem)
            {
                updateSystems.Add(updateSystem);
            }

            if (system is IDisposeSystem destroySystem)
            {
                destroySystems.Add(destroySystem);
            }

        }

        public void Init()
        {

            updateSystemsArray = updateSystems.ToArray();

            InitSystems();

        }

        public void InitSystems()
        {
            foreach (var system in initSystems)
            {
                system.Init(this);
            }
        }

        public void UpdateSystems()
        {
            foreach (var system in updateSystems)
            {
                system.Update(this);
            }
        }

        internal void DestroySystems()
        {
            foreach (var system in destroySystems)
            {
                system.Dispose(this);
            }
        }


        public void AddSharedData(ISharedData sharedObject)
        {
            _sharedData.Add(sharedObject);
        }

        public object GetSharedData<T>() where T : struct
        {
            foreach (var item in _sharedData)
            {
                if (item is T) return item;
            }
            return null;
        }

        public ECSWorld GetWorld()
        {
            return GameWorld;
        }
    }
}

