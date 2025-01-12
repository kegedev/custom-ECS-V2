using Unity.Collections;
using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using System.Reflection;
using UnityEngine;

namespace Game.ECS.Base.Components
{
    public class ComponentContainer<T> where T : struct
    {
        public NativeArray<int> EntityIds;
        public NativeArray<T> Components; 
        public int EntityCount; 

        public ComponentContainer(int initialCapacity = 128)
        {
            EntityIds = new NativeArray<int>(initialCapacity, Allocator.Persistent);
            Components = new NativeArray<T>(initialCapacity, Allocator.Persistent);
            EntityCount = 0;
        }

        public void AddComponent(int entityId, T component)
        {
            if (EntityCount >= EntityIds.Length)
            {
                ResizeComponentArray();
            }

            EntityIds[EntityCount] = entityId;
            Components[EntityCount] = component;
            EntityCount++;
        }

        public unsafe ref T GetComponent(int entityId)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                if (EntityIds[i] == entityId)
                {
                    return ref UnsafeUtility.ArrayElementAsRef<T>(Components.GetUnsafePtr(), i);
                }
            }
            throw new Exception("Entity ID not found in the container " + entityId);
        }
      
        public bool HasComponent(int entityId)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                if (EntityIds[i] == entityId)
                {
                    return true;
                }
            }
            return false;
        }


        public void RemoveComponent(int entityId)
        {
            for (int i = 0; i < EntityCount; i++)
            {
                if (EntityIds[i] == entityId)
                {

                    EntityIds[i] = EntityIds[EntityCount - 1];
                    Components[i] = Components[EntityCount - 1];
                    EntityCount--;
                    return;
                }
            }

            throw new Exception($"Entity ID {entityId} not found in the container.");
        }

        private void ResizeComponentArray()
        {
            int newSize = EntityIds.Length * 2;
            NativeArray<int> newEntityIds = new NativeArray<int>(newSize, Allocator.Persistent);
            NativeArray<T> newComponents = new NativeArray<T>(newSize, Allocator.Persistent);

            NativeArray<int>.Copy(EntityIds, newEntityIds, EntityCount);
            NativeArray<T>.Copy(Components, newComponents, EntityCount);

            EntityIds.Dispose();
            Components.Dispose();

            EntityIds = newEntityIds;
            Components = newComponents;
        }

        public void Dispose()
        {
            if (EntityIds.IsCreated)
            {
                EntityIds.Dispose();
            }

            if (Components.IsCreated)
            {
                Components.Dispose();
            }
        }

    }
}
