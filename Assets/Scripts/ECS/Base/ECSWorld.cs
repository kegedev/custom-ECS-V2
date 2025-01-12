using Game.ECS.Base.Components;
using Game.Factory;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Game.ECS.Base
{
    public class ECSWorld
    {
        public int EntityIdCounter;
        public int EntityCount;
        public NativeArray<int> Entities; //tüm entitylerin IDleri
        public Dictionary<Type, object> ComponentContainers; //<componentMask,ComponentContainer>

        public Action<int> DisposeArea;
        public Action<CoordinateComponent, int> SetOccupant;
        private FactoryManager _factoryManager;

        public QuadTreeData QuadTreeData = new QuadTreeData()
        {
            QuadTreeNodeDatas = new NativeList<QuadTreeNodeData>(Allocator.Persistent),
            QuadtreeNodeIndexes = new NativeList<int>(Allocator.Persistent),
            QuadtreeLeafIndexes = new NativeList<int>(Allocator.Persistent),
            QuadtreeNodeIndex = 0,
        };
   


        public ECSWorld(int initialEntityCount,FactoryManager factoryManager)
        {
            Entities = new NativeArray<int>(initialEntityCount, Allocator.Persistent);
            ComponentContainers = new Dictionary<Type, object>();
            _factoryManager=factoryManager;
        }

        public void AddComponentToEntity<T>(int entityId, object component) where T : struct
        {
            if (component == null)
                throw new Exception("Component cannot be null.");

            if (!ComponentContainers.TryGetValue(typeof(T), out var componentContainer))
            {
                componentContainer = new ComponentContainer<T>();
                ComponentContainers.Add(typeof(T), componentContainer);
            }

            ((ComponentContainer<T>)componentContainer).AddComponent(entityId, (T)component);
        }

        public int CreateNewEntity()
        {
            if (EntityCount >= Entities.Length)
            {
                ResizeEntityArray(ref Entities);
            }

            int newEntityId = EntityIdCounter++;
            Entities[EntityCount] = newEntityId;
            EntityCount++;

            return newEntityId;
        }

        public void DisposeEntity(int entityId)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                if (Entities[i] == entityId)
                {
                    SetOccupant.Invoke(GetComponent<CoordinateComponent>(entityId), entityId);

                    if (GetComponentContainer<AreaComponent>().HasComponent(entityId)) 
                    {
                        DisposeArea.Invoke(entityId);
                        RemoveComponent<AreaComponent>(entityId);
                    }
                    if (GetComponentContainer<HealthComponent>().HasComponent(entityId)) RemoveComponent<HealthComponent>(entityId);
                    if (GetComponentContainer<CoordinateComponent>().HasComponent(entityId)) RemoveComponent<CoordinateComponent>(entityId);
                    if (GetComponentContainer<MoverComponent>().HasComponent(entityId)) RemoveComponent<MoverComponent>(entityId);
                    if (GetComponentContainer<RenderComponent>().HasComponent(entityId)) RemoveComponent<RenderComponent>(entityId);
                    if (GetComponentContainer<SoldierComponent>().HasComponent(entityId)) RemoveComponent<SoldierComponent>(entityId);
                    if (GetComponentContainer<AttackComponent>().HasComponent(entityId)) RemoveComponent<AttackComponent>(entityId);
                    if (GetComponentContainer<TileComponent>().HasComponent(entityId)) RemoveComponent<TileComponent>(entityId);
                    if (GetComponentContainer<QuadTreeLeafComponent>().HasComponent(entityId)) RemoveComponent<QuadTreeLeafComponent>(entityId);
                    if (GetComponentContainer<BuildingComponent>().HasComponent(entityId)) RemoveComponent<BuildingComponent>(entityId);

                    Entities[i] = Entities[EntityCount - 1];
                    EntityCount--;
                    return;
                }
            }
            throw new System.Exception($"Entity ID {entityId} not found.");
        }
        void RemoveComponent<T>(int entityId) where T : struct
        {
            var container = GetComponentContainer<T>();
            if (container.HasComponent(entityId))
            {
                _factoryManager.ReleaseInstance(GetComponent<T>(entityId));
                container.RemoveComponent(entityId);
            }
        }


        public void ResizeEntityArray(ref NativeArray<int> entities)
        {
            int newSize = entities.Length * 2;
            NativeArray<int> newEntities = new NativeArray<int>(newSize, Allocator.Persistent);
            NativeArray<int>.Copy(entities, newEntities, entities.Length);
            entities.Dispose();

            entities = newEntities;
        }

        public ComponentContainer<T> GetComponentContainer<T>() where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                return (ComponentContainer<T>)ComponentContainers[typeof(T)];
            }
            else
            {
                throw new System.Exception($"ComponentContainers does not include key: {typeof(T)}");
            }
        }

        public ref T GetComponent<T>(int entityId) where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                return ref ((ComponentContainer<T>)ComponentContainers[typeof(T)]).GetComponent(entityId);
            }
            else
            {
                throw new System.Exception($"ComponentContainers does not include key: {typeof(T)}");
            }
        }

        public bool HasComponentContainer<T>()
        {
            return ComponentContainers.ContainsKey(typeof(T));
        }

        public bool HasComponent<T>(int entityId) where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                return ((ComponentContainer<T>)ComponentContainers[typeof(T)]).HasComponent(entityId);
            }
            else
            {
                throw new System.Exception($"ComponentContainers does not include key: {typeof(T)}");
            }
        }
    }

}
