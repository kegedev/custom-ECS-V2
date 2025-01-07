using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RenderSystem : IInitSystem, IUpdateSystem
{
    Mesh[] meshes;
    Material[] materials;

    ECSWorld _world;
    public void Init(SystemManager systemManager)
    {
        _world = systemManager.GetWorld();
        meshes = ((MeshContainer)systemManager.GetSharedData<MeshContainer>()).Meshes;
        materials = ((MaterialContainer)systemManager.GetSharedData<MaterialContainer>()).Materials;

    }
    public void Update(SystemManager systemManager)
    {

        RenderEntities(((ComponentContainer<RenderComponent>)_world.ComponentContainers[ComponentMask.RenderComponent]).Components);
    }

    private void RenderEntities(NativeArray<RenderComponent> renderComponents)
    {
        const int batchSize = 1022;
        Matrix4x4[] batch = new Matrix4x4[batchSize];
        int batchCount = 0;
 
        for (int i = 0; i < renderComponents.Length; i++)
        {
       
            batch[batchCount++] = renderComponents[i].TRS;

            if (batchCount == batchSize)
            {
                Graphics.DrawMeshInstanced(meshes[0], 0, materials[0], batch);
                batchCount = 0;
            }
        }

    
        if (batchCount > 0)
        {
            Graphics.DrawMeshInstanced(meshes[0], 0, materials[0], batch);
        }
    }
}
