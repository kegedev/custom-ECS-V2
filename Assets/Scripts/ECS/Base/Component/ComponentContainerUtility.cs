using Unity.Collections;

namespace Game.ECS.Base.Component
{
    internal static class ComponentContainerUtility
    {
        internal static void AddComponent<T>(ref ComponentContainer<T> componentContainer, int entityId, T component) where T : struct
        {
            if (componentContainer.EntityCount >= componentContainer.EntityIds.Length)
            {
                ResizeComponentArray(ref componentContainer);
            }

            componentContainer.EntityIds[componentContainer.EntityCount] = entityId;
            componentContainer.Components[componentContainer.EntityCount] = component;
            componentContainer.EntityCount++;
        }

        internal static T GetComponent<T>(ref ComponentContainer<T> componentContainer, int entityId) where T : struct
        {
            for (int i = 0; i < componentContainer.EntityCount; i++)
            {
                if (componentContainer.EntityIds[i] == entityId)
                {
                    return componentContainer.Components[i];
                }
            }

            throw new System.Exception($"Entity ID {entityId} not found in the container.");
        }

        internal static void RemoveComponent<T>(ref ComponentContainer<T> componentContainer, int entityId) where T : struct
        {
            for (int i = 0; i < componentContainer.EntityCount; i++)
            {
                if (componentContainer.EntityIds[i] == entityId)
                {
                    componentContainer.EntityIds[i] = componentContainer.EntityIds[componentContainer.EntityCount - 1];
                    componentContainer.Components[i] = componentContainer.Components[componentContainer.EntityCount - 1];
                    componentContainer.EntityCount--;
                    return;
                }
            }

            throw new System.Exception($"Entity ID {entityId} not found in the container.");
        }

        internal static void ResizeComponentArray<T>(ref ComponentContainer<T> componentContainer) where T : struct
        {
            int newSize = componentContainer.EntityIds.Length * 2;
            NativeArray<int> newEntityIds = new NativeArray<int>(newSize, Allocator.Persistent);
            NativeArray<T> newComponents = new NativeArray<T>(newSize, Allocator.Persistent);

            NativeArray<int>.Copy(componentContainer.EntityIds, newEntityIds, componentContainer.EntityCount);
            NativeArray<T>.Copy(componentContainer.Components, newComponents, componentContainer.EntityCount);

            componentContainer.EntityIds.Dispose();
            componentContainer.Components.Dispose();

            componentContainer.EntityIds = newEntityIds;
            componentContainer.Components = newComponents;
        }
    }
}



