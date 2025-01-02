using System.Collections.Generic;
using UnityEngine;


namespace Game.ECS.Base.Systems
{
    internal class SystemManager
    {
        private ECSWorld GameWorld;

        private List<ISystem> allSystems;
        private List<IInitSystem> initSystems;
        private List<IUpdateSystem> updateSystems;
        private List<IDisposeSystem> destroySystems;
        private List<ISharedData> _sharedData;

        private IUpdateSystem[] updateSystemsArray;


        internal SystemManager(ECSWorld gameWorld)
        {
            GameWorld = gameWorld;

            initSystems = new List<IInitSystem>();
            updateSystems = new List<IUpdateSystem>();
            destroySystems = new List<IDisposeSystem>();
            _sharedData = new List<ISharedData>();
        }

        internal void AddSystem(ISystem system)
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

        internal void Init()
        {

            updateSystemsArray = updateSystems.ToArray();

            InitSystems();

        }

        internal void InitSystems()
        {
            foreach (var system in initSystems)
            {
                system.Init(this);
            }
        }

        internal void UpdateSystems()
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


        internal void AddSharedData(ISharedData sharedObject)
        {
            _sharedData.Add(sharedObject);
        }

        internal object GetSharedData<T>() where T : struct
        {
            foreach (var item in _sharedData)
            {
                if (item is T) return item;
            }
            return null;
        }

        internal ECSWorld GetWorld()
        {
            return GameWorld;
        }
    }
}

