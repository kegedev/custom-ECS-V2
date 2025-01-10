using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.Systems
{
    public class SelectionSystem : IInitSystem
    {
        public ECSWorld _world;
        public int SelectedMoverID = -1;
        public int SelectedBuildingID = -1;

        public Func<int, int2, int2, NativeArray<int2>> GetMoverPath;

        public void Init(SystemManager systemManager)
        {
            _world = systemManager.GetWorld();
        }

        public void ProcessSelection(int selectedTileId)
        {
            Debug.Log("SelectedTile  " + selectedTileId);
            int occupantEntityId = ((ComponentContainer<TileComponent>)_world.ComponentContainers[ComponentMask.TileComponent]).GetComponent(selectedTileId).OccupantEntityID;

            if (((ComponentContainer<MoverComponent>)_world.ComponentContainers[ComponentMask.MoverComponent]).HasComponent(occupantEntityId))
            {
                if (SelectedMoverID == -1)
                {
                    SetSelectedMover(occupantEntityId);
                }
                else SetMoverPath(selectedTileId);
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

        public void ResetSelectedMoverIndex()
        {
            SelectedMoverID = -1;
        }


    }
}
