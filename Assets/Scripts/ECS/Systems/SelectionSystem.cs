using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.ECS.Systems
{
    public class SelectionSystem
    {
        public ECSWorld _world;
        public int SelectedMoverID = -1;
        public int SelectedBuildingID = -1;

        public Action< int, int> SetMoverPath;
        public Action TryToConstruct;
        public Action<SoldierType, int, int> SoldierSelected;
        public Action<BuildingType, int> BuildingSelected;
        
        public SelectionSystem(ECSWorld world)
        {
            _world = world;
        }

        public void ProcessSelection(int selectedTileId, GameState gameState)
        {
            //Debug.Log("SelectedTile  " + selectedTileId);
            if (gameState == GameState.Construction)
            {
                TryToConstruct.Invoke();
            }

            int occupantEntityId = _world.GetComponent<TileComponent>(selectedTileId).OccupantEntityID;
            int2 selectedTileCoordinate = _world.GetComponent<CoordinateComponent>(selectedTileId).Coordinate;

            if (_world.HasComponentContainer<MoverComponent>() && _world.HasComponent<MoverComponent>(occupantEntityId))
            {
                if (SelectedMoverID == -1)
                {
                    SetSelectedMover(occupantEntityId);

                    ref var soldierComponent = ref _world.GetComponent<SoldierComponent>(SelectedMoverID);
                    ref var healthComponent = ref _world.GetComponent<HealthComponent>(SelectedMoverID);
                    ref var damageComponent = ref _world.GetComponent<AttackComponent>(SelectedMoverID);

                    SoldierSelected.Invoke((SoldierType)soldierComponent.SoldierType, healthComponent.Health, damageComponent.Damage);
                }
                else
                {
                    int closestFreeNeighbour = QuerySystem.GetClosestUnoccupiedNeighbour(selectedTileCoordinate, _world);

                    ref var attackComponent = ref _world.GetComponent<AttackComponent>(SelectedMoverID);
                    ref var tileComponent = ref _world.GetComponent<TileComponent>(selectedTileId);
                    attackComponent.TargetId = tileComponent.OccupantEntityID;
                    //_world.UpdateComponent(SelectedMoverID, attackComponent);
                    SetMoverPath.Invoke( closestFreeNeighbour,SelectedMoverID);
                    ResetSelectedMoverIndex();
                    // SetMoverPath(closestFreeNeighbour);
                }
            }
            else if (SelectedMoverID == -1 && occupantEntityId != -1)
            {
                SelectedBuildingID = occupantEntityId;
                var buildingComponent = _world.GetComponent<BuildingComponent>(SelectedBuildingID);
                var healthComponent = _world.GetComponent<HealthComponent>(SelectedBuildingID);

                BuildingSelected.Invoke((BuildingType)buildingComponent.BuildingType, healthComponent.Health);
                // Debug.Log("BUILDING SELECTED");
            }
            else if (SelectedMoverID != -1 && occupantEntityId != -1)
            {
                //Debug.Log("MOVE  TO BUILDING");
                ref var attackComponent = ref _world.GetComponent<AttackComponent>(SelectedMoverID);
                ref var tileComponent = ref _world.GetComponent<TileComponent>(selectedTileId);


                attackComponent.TargetId = tileComponent.OccupantEntityID;
                //_world.UpdateComponent(SelectedMoverID, attackComponent);

                int closestFreeNeighbour = QuerySystem.GetClosestUnoccupiedNeighbourOfArea(_world, selectedTileCoordinate, 5, 5);
                SetMoverPath.Invoke( closestFreeNeighbour, SelectedMoverID);
                ResetSelectedMoverIndex();
            }
            else if (SelectedMoverID != -1)
            {
                // Debug.Log("Tile has no occupant");
                SetMoverPath.Invoke( selectedTileId, SelectedMoverID);
                ResetSelectedMoverIndex();
            }

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
