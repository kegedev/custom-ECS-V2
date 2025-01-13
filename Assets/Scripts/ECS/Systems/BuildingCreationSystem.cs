using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.Factory;
using System;
using Unity.Mathematics;
using UnityEngine;


namespace Game.ECS.Systems
{
    public class BuildingCreationSystem
    {
        private ECSWorld _world;
        private FactoryManager _factoryManager;
        public Action<int,int2,BuildingType> CreateArea;
        public Action<int> DisposeArea;

        public BuildingCreationSystem(ECSWorld world,FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
            _world = world;
        }


        public void CreateBuilding(BuildingType buildingType, int rootTileId)
        {

            int newEntityID = _world.CreateNewEntity();

            var rootTileCoord = _world.GetComponent<CoordinateComponent>(rootTileId);

            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(rootTileCoord.Coordinate);

            _world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                            coordinateComponent);
            _world.AddComponentToEntity<AreaComponent>(newEntityID,
                                                          _factoryManager.GetInstance<AreaComponent>(new object[] { MapConstants.BuildingSize.x, MapConstants.BuildingSize.y }));
            _world.AddComponentToEntity<BuildingComponent>(newEntityID,
                                                          _factoryManager.GetInstance<BuildingComponent>(new object[] { buildingType }));
            _world.AddComponentToEntity<HealthComponent>(newEntityID,
                                                  _factoryManager.GetInstance<HealthComponent>(new object[] { MapConstants.BuildingHealth[buildingType] }));
            CreateArea.Invoke(newEntityID, rootTileCoord.Coordinate, buildingType);
        }
    }

}
