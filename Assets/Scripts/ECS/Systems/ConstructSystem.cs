using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.LightTransport;
using Unity.Collections;
using static UnityEditor.Progress;

namespace Game.ECS.Systems
{
    public class ConstructSystem : IInitSystem, IUpdateSystem
    {
   
        public Func<Vector2> GetInputPos;
        int currentTileId=-1;
        private NativeArray<int> _previewTileIds;
        private NativeArray<float2> _previousOffsets;
        public void Init(SystemManager systemManager)
        {
            _previewTileIds = new NativeArray<int>(25,Allocator.Persistent);
            _previousOffsets=new NativeArray<float2>(25,Allocator.Persistent);
            
        }

        public void Update(SystemManager systemManager)
        {
            ECSWorld world=systemManager.GetWorld();
            int tileEntityId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)world.ComponentContainers[ComponentMask.QuadTreeLeafComponent],
                                    world.quadTreeNodeDatas,
                                    world.QuadtreeNodeIndexes,
                                    world.QuadtreeLeafIndexes,
                                    world.TileQuadtreeRoot,
                                    GetInputPos.Invoke());

            if(tileEntityId!= currentTileId)
            {
                if(currentTileId!=-1) ClearPreview(systemManager.GetWorld());
                currentTileId = tileEntityId;
                ShowPreview(world, CheckContructionArea(world), BuildingType.PowerPlant);


            }
        }

        private bool CheckContructionArea(ECSWorld world)
        {
            bool isAreafree = true;
            int counter = 0;
            var renderComponentContainer = world.GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
            var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
            var tileComponentContainer = world.GetComponentContainer<TileComponent>(ComponentMask.TileComponent);
            int2 coordinate = coordinateComponentContainer.GetComponent(currentTileId).Coordinate;
            for (int w = 0; w < 5; w++)
            {
                for (int h = 0; h < 5; h++)
                {
                    int pcAbsoluteX = coordinate.x + w;
                    int pcAbsoluteY = coordinate.y + h;

                    if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                        continue;
                    int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(ComponentMask.QuadTreeLeafComponent),
                                                   world.quadTreeNodeDatas,
                                                   world.QuadtreeNodeIndexes,
                                                   world.QuadtreeLeafIndexes,
                                                   world.TileQuadtreeRoot,
                                                   new Vector2(pcAbsoluteX, pcAbsoluteY));
                    var renderComp = renderComponentContainer.GetComponent(tileId);
                    var coordinateComp = coordinateComponentContainer.GetComponent(tileId);
                    var tileComp = tileComponentContainer.GetComponent(tileId);
                    _previewTileIds[counter]=tileId;
                    _previousOffsets[counter++] = renderComp.TextureOffset;
                    if (tileComp.OccupantEntityID!=-1) isAreafree = false;
                    renderComp.TextureOffset = new float2(0.5f, 0.5f);
                    renderComponentContainer.UpdateComponent(tileId, renderComp);
  
                }
            }

            return isAreafree;
        }

        private void ShowPreview(ECSWorld world, bool isAreaFree, BuildingType buildingType)
        {
            var renderComponentContainer = world.GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
      
            for (int i = 0; i < 25; i++)
            {
                var renderComp = renderComponentContainer.GetComponent(_previewTileIds[i]);
         
                renderComp.TextureOffset = MapConstants.BuildingOffsets[isAreaFree?buildingType:BuildingType.PreviewRed];
                renderComponentContainer.UpdateComponent(_previewTileIds[i], renderComp);
            }
    
        }
        private void ClearPreview(ECSWorld world)
        {
            int counter = 0;
            var renderComponentContainer = world.GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
          
            for (int i = 0; i < 25; i++)
            {
                var renderComp = renderComponentContainer.GetComponent(_previewTileIds[i]);
            
                renderComp.TextureOffset = _previousOffsets[counter++];
                renderComponentContainer.UpdateComponent(_previewTileIds[i], renderComp);
            }
        }
    }
}