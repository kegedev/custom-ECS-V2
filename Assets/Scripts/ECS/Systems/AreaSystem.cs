using Game.ECS.Base;
using Game.ECS.Base.Components;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;

public class AreaSystem : MonoBehaviour
{

    private ECSWorld _world;
    public Action<CoordinateComponent, int> SetOccupant;
    public Func<CoordinateComponent, int> GetOccupant;

    public AreaSystem(ECSWorld world)
    {
        _world = world;
    }

    public void CreateArea(int entityId,int2 rootTileCoord,BuildingType buildingType)
    {
        for (int w = 0; w < 5; w++)
        {
            for (int h = 0; h < 5; h++)
            {
                int pcAbsoluteX = rootTileCoord.x + w;
                int pcAbsoluteY = rootTileCoord.y + h;

                if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                    continue;
                int tileId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                     _world.QuadTreeData,
                                                     new Vector2(pcAbsoluteX, pcAbsoluteY));

                ref var renderComp = ref _world.GetComponent<RenderComponent>(tileId);
                ref var coordinateComp = ref _world.GetComponent<CoordinateComponent>(tileId);

                renderComp.TextureOffset = MapConstants.BuildingOffsets[buildingType];
                SetOccupant.Invoke(coordinateComp, entityId);
            }
        }
    }
    public void DisposeArea(int entityId)
    {
        var rootTileCoord = _world.GetComponent<CoordinateComponent>(entityId);

        var areaComponent = _world.GetComponent<AreaComponent>(entityId);
        for (int w = 0; w < areaComponent.Width; w++)
        {
            for (int h = 0; h < areaComponent.Height; h++)
            {
                int pcAbsoluteX = rootTileCoord.Coordinate.x + w;
                int pcAbsoluteY = rootTileCoord.Coordinate.y + h;

                if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                    continue;
                int tileId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                     _world.QuadTreeData,
                new Vector2(pcAbsoluteX, pcAbsoluteY));

                ref var renderComp = ref _world.GetComponent<RenderComponent>(tileId);
                ref var coordinateComp = ref _world.GetComponent<CoordinateComponent>(tileId);


                renderComp.TextureOffset = ((pcAbsoluteX + pcAbsoluteY) % 2 == 0) ? MapConstants.TerrainOffsets[TerrainType.LightGreen] : MapConstants.TerrainOffsets[TerrainType.DarkGreen];
                // world.UpdateComponent(tileId, renderComp);
                SetOccupant.Invoke(coordinateComp, -1);
            }
        }
    }
}
