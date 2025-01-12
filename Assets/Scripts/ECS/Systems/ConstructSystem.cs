using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Game.ECS.Systems
{
    public class ConstructSystem: IUpdateSystem
    {
        private ECSWorld _world;
        public Func<Vector2> GetInputPos;
        private int currentTileId = -1;
        private List<int> _previewTileIds;
        private List<float2> _previousOffsets;
        private BuildingType _buildingType = BuildingType.None;

        public Action<GameState> SetGameState;
        public Action<BuildingType, int> ConstructBuilding;
        public ushort ActiveStateMask => (ushort)GameState.Construction;
        private bool isAreF = false;

        public ConstructSystem(ECSWorld world)
        {
            _world = world;
            _previewTileIds = new List<int>();
            _previousOffsets = new List<float2>();
        }
     
        public void Update(SystemManager systemManager)
        {

            ECSWorld world = systemManager.GetWorld();
            int tileEntityId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                       world.QuadTreeData,
                                                       GetInputPos.Invoke());

            if (tileEntityId != currentTileId)
            {
                if (currentTileId != -1) ClearPreview(systemManager.GetWorld());
                currentTileId = tileEntityId;
                isAreF = CheckContructionArea(world);
                ShowPreview(world, isAreF);


            }


        }

        public void TryToConstruct()
        {
            if (isAreF)
            {
                SetGameState.Invoke(GameState.MainState);
                if (currentTileId != -1) ClearPreview(_world);
                _previousOffsets.Clear();
                _previewTileIds.Clear();
                ConstructBuilding.Invoke(_buildingType, currentTileId);
            }
        }

        private bool CheckContructionArea(ECSWorld world)
        {
            bool isAreafree = true;

            int2 coordinate = world.GetComponent<CoordinateComponent>(currentTileId).Coordinate;
            for (int w = 0; w < 5; w++)
            {
                for (int h = 0; h < 5; h++)
                {
                    int pcAbsoluteX = coordinate.x + w;
                    int pcAbsoluteY = coordinate.y + h;

                    if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                        return false;
                    int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                   world.QuadTreeData,
                                                   new Vector2(pcAbsoluteX, pcAbsoluteY));

                    ref var renderComp = ref world.GetComponent<RenderComponent>(tileId);
                    ref var coordinateComp = ref world.GetComponent<CoordinateComponent>(tileId);
                    ref var tileComp = ref world.GetComponent<TileComponent>(tileId);
                    _previewTileIds.Add(tileId);
                    _previousOffsets.Add(renderComp.TextureOffset);
                    if (tileComp.OccupantEntityID != -1) isAreafree = false;
                    renderComp.TextureOffset = new float2(0.5f, 0.5f);
                 
                }
            }
            return isAreafree;
        }

        private void ShowPreview(ECSWorld world, bool isAreaFree)
        {
            foreach (var previewTileId in _previewTileIds)
            {
                ref var renderComp = ref world.GetComponent<RenderComponent>(previewTileId);
                renderComp.TextureOffset = MapConstants.BuildingOffsets[isAreaFree ? _buildingType : BuildingType.PreviewRed];
               // world.UpdateComponent(previewTileId, renderComp);
            }
        }
        private void ClearPreview(ECSWorld world)
        {
            for (int i = 0; i < _previewTileIds.Count; i++)
            {
                ref var renderComp = ref world.GetComponent<RenderComponent>(_previewTileIds[i]);

                renderComp.TextureOffset = _previousOffsets[i];
                //world.UpdateComponent(_previewTileIds[i], renderComp);
            }
            _previousOffsets.Clear();
            _previewTileIds.Clear();
        }
        public void BuildingSelectedToConstruct(BuildingType buildingType)
        {
            _buildingType = buildingType;
            SetGameState.Invoke(GameState.Construction);
        }
    }
}