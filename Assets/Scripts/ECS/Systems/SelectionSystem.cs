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
            //Debug.Log(occupantEntityId);

            ////  Debug.Log("Selected Mover index "+ SelectedMoverIndex);
        }



        public void SetMoverPath(int targetTileId)//int moverIndex, int2 tagretCoordinate
        {
            Debug.Log("Set mover Path");
            //Profiler.BeginSample("SetMoverPath");
            ComponentContainer<CoordinateComponent> coordinateCompContainer = (ComponentContainer<CoordinateComponent>)_world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
            int2 startCoord = coordinateCompContainer.GetComponent(SelectedMoverID).Coordinate;
            int2 targetCoord = coordinateCompContainer.GetComponent(targetTileId).Coordinate;
            NativeArray <int2> path = GetMoverPath.Invoke(SelectedMoverID, startCoord, targetCoord);
            Debug.Log(startCoord +" "+ targetCoord + " PATH-> "+path.Length);
            if (path.Length == 0)
            { ResetSelectedMoverIndex(); return; }

            //ChunkUtility.GetEntityComponentValueAtIndex<MoverComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex).Path = path;
            //ChunkUtility.GetEntityComponentValueAtIndex<MoverComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex).HasPath = true;

            ResetSelectedMoverIndex();
            //Profiler.EndSample();
        }

        public void SetSelectedMover(int selectedMoverIndex)
        {
            SelectedMoverID = selectedMoverIndex;
            Debug.Log("SelectedTile occupant " + SelectedMoverID);
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
