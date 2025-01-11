using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Game.ECS.Systems
{
    public class SelectionSystem : IInitSystem
    {
        public ECSWorld _world;
        public int SelectedMoverID = -1;
        public int SelectedBuildingID = -1;

        public Func<int, int2, int2, NativeArray<int2>> GetMoverPath;
        public Action TryToConstruct;
        public Action<SoldierType , int ,int> SoldierSelected;
        public Action<BuildingType , int > BuildingSelected;
        public void Init(SystemManager systemManager)
        {
            _world = systemManager.GetWorld();
        }

        public void ProcessSelection(int selectedTileId,GameState gameState)
        {
            Debug.Log("SelectedTile  " + selectedTileId);
            if(gameState==GameState.Construction)
            {
                TryToConstruct.Invoke();
            }
            int occupantEntityId = ((ComponentContainer<TileComponent>)_world.ComponentContainers[ComponentMask.TileComponent]).GetComponent(selectedTileId).OccupantEntityID;
            int2 selectedTileCoordinate= ((ComponentContainer<CoordinateComponent>)_world.ComponentContainers[ComponentMask.CoordinateComponent]).GetComponent(selectedTileId).Coordinate;
            if (_world.ComponentContainers.ContainsKey(ComponentMask.MoverComponent) && ((ComponentContainer<MoverComponent>)_world.ComponentContainers[ComponentMask.MoverComponent]).HasComponent(occupantEntityId))
            {
                if (SelectedMoverID == -1)
                {
                    SetSelectedMover(occupantEntityId);
                    var soldierComponentContainer = (ComponentContainer<SoldierComponent>)_world.ComponentContainers[ComponentMask.SoldierComponent];
                    var healthComponentContainer = (ComponentContainer<HealthComponent>)_world.ComponentContainers[ComponentMask.HealthComponent];
                    var damageComponentContainer = (ComponentContainer<DamageComponent>)_world.ComponentContainers[ComponentMask.DamageComponent];
                    var soldierComponent = soldierComponentContainer.GetComponent(SelectedMoverID);
                    var healthComponent = healthComponentContainer.GetComponent(SelectedMoverID);
                    var damageComponent = damageComponentContainer.GetComponent(SelectedMoverID);
                    SoldierSelected.Invoke((SoldierType)soldierComponent.SoldierType, healthComponent.Health, damageComponent.Damage);
                }
                else
                {
                   int closestFreeNeighbour= QuerySystem.GetClosestUnoccupiedNeighbour(selectedTileCoordinate, _world);
                    SetMoverPath(closestFreeNeighbour);
                }
            }else if(SelectedMoverID == -1 && occupantEntityId != -1)
            {
                SelectedBuildingID = occupantEntityId;
                var buildingComponentContainer=(ComponentContainer<BuildingComponent>)_world.ComponentContainers[ComponentMask.BuildingComponent];
                var buildingComponent= buildingComponentContainer.GetComponent(SelectedBuildingID);
                var healthComponentContainer = (ComponentContainer<HealthComponent>)_world.ComponentContainers[ComponentMask.HealthComponent];
                var healthComponent = healthComponentContainer.GetComponent(SelectedBuildingID);
                BuildingSelected.Invoke((BuildingType)buildingComponent.BuildingType, healthComponent.Health);
                Debug.Log("BUILDING SELECTED");
            }else if(SelectedMoverID != -1 && occupantEntityId != -1)
            {
                Debug.Log("MOVE  TO BUILDING");
                int closestFreeNeighbour = QuerySystem.GetClosestUnoccupiedNeighbourOfArea(_world, selectedTileCoordinate, 5, 5);
                SetMoverPath(closestFreeNeighbour);
            }
            else if(SelectedMoverID != -1)
            {
                Debug.Log("Tile has no occupant");
                SetMoverPath(selectedTileId);
            }
   
        }



        public void SetMoverPath(int targetTileId)
        {
          
            var coordinateCompContainer = _world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
            int2 startCoord = coordinateCompContainer.GetComponent(SelectedMoverID).Coordinate;
            int2 targetCoord = coordinateCompContainer.GetComponent(targetTileId).Coordinate;
            NativeArray <int2> path = GetMoverPath.Invoke(SelectedMoverID, startCoord, targetCoord);
            
            if (path.Length == 0)
            { ResetSelectedMoverIndex(); return; }

            var moverComponentContainer = _world.GetComponentContainer<MoverComponent>(ComponentMask.MoverComponent);
            var moverComponent = moverComponentContainer.GetComponent(SelectedMoverID);

            moverComponent.Path = path;
            moverComponent.HasPath = true;
         
            moverComponentContainer.UpdateComponent(SelectedMoverID,moverComponent);

            ResetSelectedMoverIndex();
        }

        public void SetSelectedMover(int selectedMoverIndex)
        {
            SelectedMoverID = selectedMoverIndex;
        }
        public int GetSelectedMoverIndex()
        {
            return SelectedMoverID;
        }
        public int GetSelectedBuildingId()
        {
            return SelectedBuildingID;
        }

        public void ResetSelectedMoverIndex()
        {
            SelectedMoverID = -1;
        }



    }
}
