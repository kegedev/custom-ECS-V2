using System.Collections.Generic;
using Unity.Collections;
using Game.ECS.Base.Components;
using Game.Factory.Base;
using Game.Pool;
using System.ComponentModel;
using System;

namespace Game.ECS.Base
{
    public class ECSWorld
    {
        public int EntityIdCounter;
        public int EntityCount;
        public NativeArray<int> Entities; //tüm entitylerin IDleri
        public Dictionary<ComponentMask, object> ComponentContainers; //<componentMask,ComponentContainer>

        //quadtree structuna cek burayı
        public NativeList<QuadTreeNodeData> quadTreeNodeDatas = new NativeList<QuadTreeNodeData>(Allocator.Persistent);
        public NativeList<int> QuadtreeNodeIndexes = new NativeList<int>(Allocator.Persistent);
        public NativeList<int> QuadtreeLeafIndexes = new NativeList<int>(Allocator.Persistent);
        public int quadtreeNodeIndex = 0;
        public QuadTreeNodeData TileQuadtreeRoot;

        public ECSWorld(int initialEntityCount) 
        {
            Entities = new NativeArray<int>(initialEntityCount, Allocator.Persistent);
            ComponentContainers= new Dictionary<ComponentMask, object>();
        }

        public void AddComponentToEntity<T>(int entityId, ComponentMask componentMask, object component) where T : struct
        {
            if (component == null)
                throw new Exception("Component cannot be null.");

            if (!ComponentContainers.TryGetValue(componentMask, out var componentContainer))
            {
                componentContainer = new ComponentContainer<T>();
                ComponentContainers.Add(componentMask, componentContainer);
            }

            ((ComponentContainer<T>)componentContainer).AddComponent(entityId, (T)component);
        }
        public T GetComponentOfEntity<T>(int entityId, ComponentMask componentMask) where T : struct
        {
            if (!ComponentContainers.TryGetValue(componentMask, out var componentContainer))
            {
                componentContainer = new ComponentContainer<T>();
                ComponentContainers.Add(componentMask, componentContainer);
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
                    Entities[i] = Entities[EntityCount - 1];
                    EntityCount--;

                    foreach (var componentContainer in ComponentContainers.Values)
                    {
                        
                        if (componentContainer is ComponentContainer<CoordinateComponent> positionContainer)
                        {
                           // ComponentContainerUtility.RemoveComponent(ref positionContainer, entityId);
                        }

                        else
                        {
                            throw new System.Exception($"Unknown component container type: {componentContainer.GetType()}");
                        }
                    }

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
            if (ComponentContainers.ContainsKey(componentMask))
            {
                throw new System.Exception($"Component container with mask {componentMask} already exists.");
            }

            var newContainer = new ComponentContainer<T>
            {
                EntityIds = new NativeArray<int>(Entities.Length, Allocator.Persistent),
                Components = new NativeArray<T>(Entities.Length, Allocator.Persistent),
                EntityCount = 0
            };

            ComponentContainers.Add(componentMask, newContainer);
        }

        public ComponentContainer<T> GetComponentContainer<T>(ComponentMask key) where T : struct
        {
            if (ComponentContainers.ContainsKey(key))
            {
                return (ComponentContainer<T>)ComponentContainers[key];
            }
            else
            {
                throw new System.Exception($"ComponentContainers does not include key: {key}");
            }
        }
    }

}
