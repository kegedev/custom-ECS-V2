using System;
using System.Collections.Generic;
using Game.Factory.Base;
using Game.Pool;



namespace Game.Factory
{
    public class FactoryManager
    {
        private PoolManager _poolManager;
        public FactoryManager(PoolManager poolManager)
        {
            _poolManager = poolManager;
            InitializeFactories();
        }

        public Dictionary<Type, object> factories = new Dictionary<Type, object>();

        public T GetInstance<T>(params object[] arguments)
        {

            if (factories.TryGetValue(typeof(T), out var factory))
            {
                return ((BaseFactory<T>)factory).GetPoolable(_poolManager.GetPool<T>(), arguments);
            }
            else
            {

                throw new Exception("No Factory Assigned "+ typeof(T));
            }

        }

        //internal void ReleaseInstance<T>(T instance)
        //{
        //    if (factories.TryGetValue(typeof(T), out var factory) && instance is IComponent poolable)
        //    {
        //        ((BaseFactory<T>)factory).ReleasePoolable(_poolManager.GetPool<T>(), poolable);
        //    }
        //    else
        //    {

        //        throw new Exception("No Factory Assigned");
        //    }
        //}

        public void AddFactory<T>(BaseFactory<T> factory) where T : new()
        {
            factories.Add(typeof(T), factory);
        }

        private void InitializeFactories()
        {
            AddFactory(new TileComponentFactory());
            AddFactory(new QuadtreeLeafComponentFactory());
            AddFactory(new RenderComponentFactory());
            AddFactory(new MoverComponentFactory());
            AddFactory(new AreaComponentFactory());
            AddFactory(new BuildingComponentFactory());
            AddFactory(new SoldierComponentFactory());
            AddFactory(new CoordinateComponentFactory());

        }
    }
}

