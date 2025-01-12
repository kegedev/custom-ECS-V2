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
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Game.ECS.Systems
{
    public class ConstructSystem : IInitSystem, IUpdateSystem
    {
        ECSWorld world;
        public Func<Vector2> GetInputPos;
        int currentTileId=-1;
        private List<int> _previewTileIds;
        private List<float2> _previousOffsets;
        private BuildingType _buildingType=BuildingType.None;

        public Action<GameState> SetGameState;
        public Action<BuildingType,int> ConstructBuilding;
        public ushort ActiveStateMask => (ushort)GameState.Construction;
        bool isAreF = false;
        public void Init(SystemManager systemManager)
        {
            world=systemManager.GetWorld();
            _previewTileIds = new List<int>();
            _previousOffsets=new List<float2>();
            
        }

        public void Update(SystemManager systemManager)
        {
          
            ECSWorld world=systemManager.GetWorld();
            int tileEntityId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)world.ComponentContainers[typeof(QuadTreeLeafComponent)],
                                    world.quadTreeNodeDatas,
                                    world.QuadtreeNodeIndexes,
                                    world.QuadtreeLeafIndexes,
                                    world.TileQuadtreeRoot,
                                    GetInputPos.Invoke());

            if(tileEntityId!= currentTileId)
            {
                if(currentTileId!=-1) ClearPreview(systemManager.GetWorld());
                currentTileId = tileEntityId;
                isAreF = CheckContructionArea(world);
                ShowPreview(world, isAreF);


            }

       
        }

        public void TryToConstruct()
        {
            Debug.Log("TryToConstruct");
            if(isAreF)
            {
                Debug.Log("TryToConstruct2 ");
                SetGameState.Invoke(GameState.MainState);
                if (currentTileId != -1) ClearPreview(world);
                _previousOffsets.Clear();
                _previewTileIds.Clear();
                Debug.Log(_buildingType);
                ConstructBuilding.Invoke(_buildingType, currentTileId);
            }
        }

        private bool CheckContructionArea(ECSWorld world)
        {
            bool isAreafree = true;

            //var renderComponentContainer = world.GetComponentContainer<RenderComponent>();
            //var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>();
            //var tileComponentContainer = world.GetComponentContainer<TileComponent>();
            int2 coordinate = world.GetComponent<CoordinateComponent>(currentTileId).Coordinate;
            //int2 coordinate = coordinateComponentContainer.GetComponent(currentTileId).Coordinate;
            for (int w = 0; w < 5; w++)
            {
                for (int h = 0; h < 5; h++)
                {
                    int pcAbsoluteX = coordinate.x + w;
                    int pcAbsoluteY = coordinate.y + h;

                    if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                        return false;
                    int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                   world.quadTreeNodeDatas,
                                                   world.QuadtreeNodeIndexes,
                                                   world.QuadtreeLeafIndexes,
                                                   world.TileQuadtreeRoot,
                                                   new Vector2(pcAbsoluteX, pcAbsoluteY));

                    var renderComp = world.GetComponent<RenderComponent>(tileId);
                    var coordinateComp = world.GetComponent<CoordinateComponent>(tileId);
                    var tileComp = world.GetComponent<TileComponent>(tileId);
                    //var renderComp = renderComponentContainer.GetComponent(tileId);
                    //var coordinateComp = coordinateComponentContainer.GetComponent(tileId);
                    //var tileComp = tileComponentContainer.GetComponent(tileId);
                    _previewTileIds.Add(tileId);
                    _previousOffsets.Add( renderComp.TextureOffset);
                    if (tileComp.OccupantEntityID!=-1) isAreafree = false;
                    renderComp.TextureOffset = new float2(0.5f, 0.5f);
                    world.UpdateComponent(tileId, renderComp);
  
                }
            }

            return isAreafree;
        }

        private void ShowPreview(ECSWorld world, bool isAreaFree)
        {
            
            foreach (var previewTileId in _previewTileIds)
            {
             var renderComp=world.GetComponent<RenderComponent>(previewTileId);
                
                renderComp.TextureOffset = MapConstants.BuildingOffsets[isAreaFree? _buildingType : BuildingType.PreviewRed];
                world.UpdateComponent(previewTileId, renderComp);
              
            }
    
        }
        private void ClearPreview(ECSWorld world)
        {
            for (int i = 0; i < _previewTileIds.Count; i++)
            {
                var renderComp = world.GetComponent<RenderComponent>(_previewTileIds[i]);
            
                renderComp.TextureOffset = _previousOffsets[i];
                world.UpdateComponent(_previewTileIds[i], renderComp);
            }
            _previousOffsets.Clear();
            _previewTileIds.Clear();
        }

        public void BuildingSelectedToConstruct(BuildingType buildingType)
        {
            _buildingType=buildingType;
            SetGameState.Invoke(GameState.Construction);
        }
    
    
    }
}