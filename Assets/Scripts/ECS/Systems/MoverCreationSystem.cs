using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.Factory;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.Systems
{
    public class MoverCreationSystem
    {
        private ECSWorld _world;
        private FactoryManager _factoryManager;
        public Action<CoordinateComponent, int> SetOccupant;
        public Func<CoordinateComponent, int> GetOccupant;
        public Func<int> GetSelectedBuildingId;

        public MoverCreationSystem(ECSWorld world,FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
            _world = world;
        }

        public void CreateMover(SoldierType soldierType)
        {
            int selectedBuildingId = GetSelectedBuildingId.Invoke();
            var buildingRootCoordinateComponent = _world.GetComponent<CoordinateComponent>(selectedBuildingId);
            int soldierSpawnTileId=QuerySystem.GetClosestUnoccupiedNeighbourOfArea(_world, buildingRootCoordinateComponent.Coordinate,5,5);
            var spawnCoordinateComponent = _world.GetComponent<CoordinateComponent>(soldierSpawnTileId);
            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(spawnCoordinateComponent.Coordinate.x, spawnCoordinateComponent.Coordinate.y, 0), Quaternion.identity, Vector3.one * 0.95f);
            int newEntityID = _world.CreateNewEntity();

            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(spawnCoordinateComponent.Coordinate);

            _world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                            coordinateComponent);
            _world.AddComponentToEntity<MoverComponent>(newEntityID,
                                                       _factoryManager.GetInstance<MoverComponent>(new object[] { false, 0, new NativeArray<int2>() }));
            _world.AddComponentToEntity<RenderComponent>(newEntityID,
                                                        _factoryManager.GetInstance<RenderComponent>(new object[2] { matrix, MapConstants.SoldierOffsets[soldierType] }));
            _world.AddComponentToEntity<SoldierComponent>(newEntityID,
                                                         _factoryManager.GetInstance<SoldierComponent>(new object[] { soldierType }));
            _world.AddComponentToEntity<HealthComponent>(newEntityID,
                                                        _factoryManager.GetInstance<HealthComponent>(new object[] { MapConstants.SoldierHealth }));
            _world.AddComponentToEntity<AttackComponent>(newEntityID,
                                                        _factoryManager.GetInstance<AttackComponent>(new object[] { MapConstants.SoldierDamages[soldierType] ,-1}));
           
            SetOccupant.Invoke(coordinateComponent, newEntityID);
        }

    }
}
