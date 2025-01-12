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

    public ushort ActiveStateMask => (ushort)(GameState.Construction | GameState.MainState);

    public void Init(SystemManager systemManager)
    {
        _world = systemManager.GetWorld();
        meshes = ((MeshContainer)systemManager.GetSharedData<MeshContainer>()).Meshes;
        materials = ((MaterialContainer)systemManager.GetSharedData<MaterialContainer>()).Materials;

    }
    public void Update(SystemManager systemManager)
    {

        RenderEntities(((ComponentContainer<RenderComponent>)_world.ComponentContainers[typeof(RenderComponent)]).Components);
    }

    private void RenderEntities(NativeArray<RenderComponent> renderComponents)//rendercomponent containerla gir buraya
    {
        const int batchSize = 1024;
        Matrix4x4[] batch = new Matrix4x4[batchSize];
        Vector4[] offsets = new Vector4[batchSize];
        int batchCount = 0;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        for (int i = 0; i < renderComponents.Length; i++)
        {
            batch[batchCount] = renderComponents[i].TRS;
           
            offsets[batchCount++] = new Vector4(0.25f, 0.5f,renderComponents[i].TextureOffset.x, renderComponents[i].TextureOffset.y);
           
            if (batchCount == batchSize)
            {
                propertyBlock.SetVectorArray("_MainTex_ST", offsets);
                Graphics.DrawMeshInstanced(meshes[0], 0, materials[0], batch, batchCount, propertyBlock);
                batchCount = 0;
            }
        }

        if (batchCount > 0)
        {
            propertyBlock.SetVectorArray("_MainTex_ST", offsets);
            Graphics.DrawMeshInstanced(meshes[0], 0, materials[0], batch, batchCount, propertyBlock);
            batchCount = 0;
        }
    }

}
