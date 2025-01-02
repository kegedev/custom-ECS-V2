using System.Collections.Generic;
using Unity.Collections;
using Game.ECS.Base.Components;

namespace Game.ECS.Base
{
    internal class ECSWorld
    {
        internal int EntityIdCounter;
        internal int EntityCount;
        internal NativeArray<int> Entities; //tüm entitylerin IDleri
        internal Dictionary<ushort, object> ComponentContainers; //<componentMask,ComponentContainer>

        internal int CreateNewEntity()
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


        internal void DisposeEntity(int entityId)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                if (Entities[i] == entityId)
                {
                    Entities[i] = Entities[EntityCount - 1];
                    EntityCount--;

                    foreach (var componentContainer in ComponentContainers.Values)
                    {
                        if (componentContainer is ComponentContainer<PositionComponent> positionContainer)
                        {
                            ComponentContainerUtility.RemoveComponent(ref positionContainer, entityId);
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


        internal void ResizeEntityArray(ref NativeArray<int> entities)
        {
            int newSize = entities.Length * 2;
            NativeArray<int> newEntities = new NativeArray<int>(newSize, Allocator.Persistent);

            NativeArray<int>.Copy(entities, newEntities, entities.Length);

            entities.Dispose();

            entities = newEntities;
        }

        public void AddComponentContainer<T>(ushort componentMask) where T : struct
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

        public ComponentContainer<T> GetComponentContainer<T>(ushort key) where T : struct
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
