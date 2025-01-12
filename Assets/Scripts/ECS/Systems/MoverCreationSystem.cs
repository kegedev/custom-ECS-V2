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
    public class MoverCreationSystem : IInitSystem
    {
        ECSWorld world;
        private FactoryManager _factoryManager;
        public Action<CoordinateComponent, int> SetOccupant;
        public Func<CoordinateComponent, int> GetOccupant;
        public Func<int> GetSelectedBuildingId;

        public MoverCreationSystem(FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
        }
        public void Init(SystemManager systemManager)
        {
            world = systemManager.GetWorld();
        }

        public void CreateMover(SoldierType soldierType)
        {
            int selectedBuildingId = GetSelectedBuildingId.Invoke();
            var buildingRootCoordinateComponent = world.GetComponent<CoordinateComponent>(selectedBuildingId);
            int soldierSpawnTileId=QuerySystem.GetClosestUnoccupiedNeighbourOfArea(world, buildingRootCoordinateComponent.Coordinate,5,5);
            var spawnCoordinateComponent = world.GetComponent<CoordinateComponent>(soldierSpawnTileId);
            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(spawnCoordinateComponent.Coordinate.x, spawnCoordinateComponent.Coordinate.y, 0), Quaternion.identity, Vector3.one * 0.95f);
            int newEntityID = world.CreateNewEntity();

            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(spawnCoordinateComponent.Coordinate);

            world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                            coordinateComponent);
            world.AddComponentToEntity<MoverComponent>(newEntityID,
                                                       _factoryManager.GetInstance<MoverComponent>(new object[] { false, 0, new NativeArray<int2>() }));
            world.AddComponentToEntity<RenderComponent>(newEntityID,
                                                        _factoryManager.GetInstance<RenderComponent>(new object[2] { matrix, MapConstants.SoldierOffsets[soldierType] }));
            world.AddComponentToEntity<SoldierComponent>(newEntityID,
                                                         _factoryManager.GetInstance<SoldierComponent>(new object[] { soldierType }));
            world.AddComponentToEntity<HealthComponent>(newEntityID,
                                                        _factoryManager.GetInstance<HealthComponent>(new object[] { MapConstants.SoldierHealth }));
            world.AddComponentToEntity<AttackComponent>(newEntityID,
                                                        _factoryManager.GetInstance<AttackComponent>(new object[] { MapConstants.SoldierDamages[soldierType] ,-1}));
           
            SetOccupant.Invoke(coordinateComponent, newEntityID);
        }

        public void DisposeMover(int moverId)
        {
            if (world.GetComponentContainer<CoordinateComponent>().HasEntity(moverId)) world.GetComponentContainer<CoordinateComponent>().RemoveComponent(moverId);
            if (world.GetComponentContainer<MoverComponent>().HasEntity(moverId))      world.GetComponentContainer<MoverComponent>().RemoveComponent(moverId);
            if (world.GetComponentContainer<RenderComponent>().HasEntity(moverId))     world.GetComponentContainer<RenderComponent>().RemoveComponent(moverId);
            if (world.GetComponentContainer<SoldierComponent>().HasEntity(moverId))    world.GetComponentContainer<SoldierComponent>().RemoveComponent(moverId);
            if (world.GetComponentContainer<HealthComponent>().HasEntity(moverId))     world.GetComponentContainer<HealthComponent>().RemoveComponent(moverId);
            if (world.GetComponentContainer<AttackComponent>().HasEntity(moverId))     world.GetComponentContainer<AttackComponent>().RemoveComponent(moverId);

        }
    }
}
