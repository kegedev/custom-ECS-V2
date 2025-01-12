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
        public Action<int> DisposeBuilding;
        private FactoryManager _factoryManager;
        

        public QuadTreeData QuadTreeData = new QuadTreeData()
        {
            QuadTreeNodeDatas = new NativeList<QuadTreeNodeData>(Allocator.Persistent),
            QuadtreeNodeIndexes = new NativeList<int>(Allocator.Persistent),
            QuadtreeLeafIndexes = new NativeList<int>(Allocator.Persistent),
            QuadtreeNodeIndex = 0,
        };
        //quadtree structuna cek burayı




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
        public T GetComponentOfEntity<T>(int entityId) where T : struct
        {
            if (!ComponentContainers.TryGetValue(typeof(T), out var componentContainer))
            {
                componentContainer = new ComponentContainer<T>();
                ComponentContainers.Add(typeof(T), componentContainer);
            }

            return ((ComponentContainer<T>)componentContainer).GetComponent(entityId);
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
                    var coordinateComp = GetComponent<CoordinateComponent>(entityId);
                    var disposedEntityTileId = QuerySystem.GetEntityId(GetComponentContainer<QuadTreeLeafComponent>(), QuadTreeData,new UnityEngine.Vector2(coordinateComp.Coordinate.x, coordinateComp.Coordinate.y));
                    
                    var tileComponent = GetComponent<TileComponent>(disposedEntityTileId);
                    tileComponent.OccupantEntityID = -1;
                    UpdateComponent(disposedEntityTileId, tileComponent);

                    if (GetComponentContainer<AreaComponent>().HasEntity(entityId))
                    {
                        DisposeBuilding.Invoke(entityId);
                        GetComponentContainer<AreaComponent>().RemoveComponent(entityId);
                    }

                    if (GetComponentContainer<CoordinateComponent>().HasEntity(entityId)) GetComponentContainer<CoordinateComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<MoverComponent>().HasEntity(entityId)) GetComponentContainer<MoverComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<RenderComponent>().HasEntity(entityId)) GetComponentContainer<RenderComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<SoldierComponent>().HasEntity(entityId)) GetComponentContainer<SoldierComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<HealthComponent>().HasEntity(entityId)) GetComponentContainer<HealthComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<AttackComponent>().HasEntity(entityId)) GetComponentContainer<AttackComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<TileComponent>().HasEntity(entityId)) GetComponentContainer<TileComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<QuadTreeLeafComponent>().HasEntity(entityId)) GetComponentContainer<QuadTreeLeafComponent>().RemoveComponent(entityId);
                    if(GetComponentContainer<BuildingComponent>().HasEntity(entityId)) GetComponentContainer<BuildingComponent>().RemoveComponent(entityId);
                   
              

                   
                    Entities[i] = Entities[EntityCount - 1];
                    EntityCount--;

                   

                    return;
                }
            }

            throw new System.Exception($"Entity ID {entityId} not found.");
        }

        

        public void ResizeEntityArray(ref NativeArray<int> entities)
        {
            int newSize = entities.Length * 2;
            NativeArray<int> newEntities = new NativeArray<int>(newSize, Allocator.Persistent);
            //newEntities.CopyFrom(Entities);
            NativeArray<int>.Copy(entities, newEntities, entities.Length);

            entities.Dispose();

            entities = newEntities;
        }

        public void AddComponentContainer<T>(ComponentMask componentMask) where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                throw new System.Exception($"Component container with mask {componentMask} already exists.");
            }

            var newContainer = new ComponentContainer<T>()
            {
                EntityIds = new NativeArray<int>(Entities.Length, Allocator.Persistent),
                Components = new NativeArray<T>(Entities.Length, Allocator.Persistent),
                EntityCount = 0,
                
            };

            ComponentContainers.Add(typeof(T), newContainer);
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

        public T GetComponent<T>(int entityId) where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                return ((ComponentContainer<T>)ComponentContainers[typeof(T)]).GetComponent(entityId);
            }
            else
            {
                throw new System.Exception($"ComponentContainers does not include key: {typeof(T)}");
            }
        }

        public void UpdateComponent<T>(int entityId, T component) where T : struct
        {
            if (ComponentContainers.ContainsKey(typeof(T)))
            {
                ((ComponentContainer<T>)ComponentContainers[typeof(T)]).UpdateComponent(entityId, component);
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
