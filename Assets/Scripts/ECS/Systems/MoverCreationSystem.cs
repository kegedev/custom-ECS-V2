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

            var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
            var buildingRootCoordinateComponent = coordinateComponentContainer.GetComponent(selectedBuildingId);
          
            Debug.Log("sourceBuildingId "+ selectedBuildingId + " "+ soldierType);

            int soldierSpawnTileId=QuerySystem.GetClosestUnoccupiedNeighbourOfArea(world, buildingRootCoordinateComponent.Coordinate,5,5);

            var spawnCoordinateComponent= coordinateComponentContainer.GetComponent(soldierSpawnTileId);
            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(spawnCoordinateComponent.Coordinate.x, spawnCoordinateComponent.Coordinate.y, 0), Quaternion.identity, Vector3.one * 0.95f);

            int newEntityID = world.CreateNewEntity();

            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(spawnCoordinateComponent.Coordinate);

            world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                               ComponentMask.CoordinateComponent,
                                                               coordinateComponent);

            world.AddComponentToEntity<MoverComponent>(newEntityID,
                                                          ComponentMask.MoverComponent,
                                                          _factoryManager.GetInstance<MoverComponent>(new object[] { false, 0, new NativeArray<int2>() }));

            world.AddComponentToEntity<RenderComponent>(newEntityID,
                                                         ComponentMask.RenderComponent,
                                                          _factoryManager.GetInstance<RenderComponent>(new object[2] { matrix, MapConstants.SoldierOffsets[soldierType] }));
            world.AddComponentToEntity<SoldierComponent>(newEntityID,
                                                  ComponentMask.SoldierComponent,
                                                  _factoryManager.GetInstance<SoldierComponent>(new object[] { soldierType }));
            SetOccupant.Invoke(coordinateComponent, newEntityID);
        }

    }

}
