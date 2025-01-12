using Game.ECS.Base.Components;
using Game.ECS.Base;
using Game.ECS.Base.Systems;
using Game.Factory;
using System;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.LightTransport;


namespace Game.ECS.Systems
{
    public class BuildingCreationSystem : IInitSystem
    {
        ECSWorld world;
        private FactoryManager _factoryManager;
        public Action<CoordinateComponent, int> SetOccupant;
        public Func<CoordinateComponent, int> GetOccupant;

        public BuildingCreationSystem(FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
        }
        public void Init(SystemManager systemManager)
        {
            world = systemManager.GetWorld();

           // CreateBuildings(eCSWorld);
        }

        public void CreateBuilding(BuildingType buildingType, int rootTileId)
        {
         
            int newEntityID = world.CreateNewEntity();

            var rootTileCoord=world.GetComponent<CoordinateComponent>(rootTileId);
     
            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(rootTileCoord.Coordinate);

            world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                            coordinateComponent);
            world.AddComponentToEntity<AreaComponent>(newEntityID,
                                                          _factoryManager.GetInstance<AreaComponent>(new object[] { 5, 5 }));
            world.AddComponentToEntity<BuildingComponent>(newEntityID,
                                                          _factoryManager.GetInstance<BuildingComponent>(new object[] { buildingType }));
            world.AddComponentToEntity<HealthComponent>(newEntityID,
                                                  _factoryManager.GetInstance<HealthComponent>(new object[] { MapConstants.BuildingHealth[buildingType] }));

            //Placeholder creation for building
            for (int w = 0; w < 5; w++)
            {
                for (int h = 0; h < 5; h++)
                {
                    int pcAbsoluteX = rootTileCoord.Coordinate.x + w;
                    int pcAbsoluteY = rootTileCoord.Coordinate.y + h;

                    if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                        continue;
                    int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                         world.quadTreeNodeDatas,
                                                         world.QuadtreeNodeIndexes,
                                                         world.QuadtreeLeafIndexes,
                                                         world.TileQuadtreeRoot,
                                                         new Vector2(pcAbsoluteX, pcAbsoluteY));

                    var renderComp = world.GetComponent<RenderComponent>(tileId);
                    var coordinateComp = world.GetComponent<CoordinateComponent>(tileId);
                       
                    
                    renderComp.TextureOffset = MapConstants.BuildingOffsets[buildingType];
                    world.UpdateComponent(tileId, renderComp);
                    SetOccupant.Invoke(coordinateComp, newEntityID);
                }
            }
        }
    }
}
