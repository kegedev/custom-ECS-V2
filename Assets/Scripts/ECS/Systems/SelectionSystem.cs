using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
public class SelectionSystem : IInitSystem
{
    public ECSWorld _world;
    public int SelectedMoverIndex = -1;

    public Func<int, int2, int2, NativeArray<int2>> GetMoverPath;

    public void Init(SystemManager systemManager)
    {
        _world = systemManager.GetWorld();
    }

    public void ProcessSelection(int2 targetTileCoord, int moverIndex)
    {
        Debug.Log(targetTileCoord);
        if (SelectedMoverIndex == -1 && moverIndex != -1)
        {
            SelectedMoverIndex = moverIndex;
        }
        else if (SelectedMoverIndex != -1 && moverIndex == -1)
        {
            SetMoverPath(SelectedMoverIndex, targetTileCoord);
        }
        else if (SelectedMoverIndex != -1 && moverIndex != -1)
        {
            //move to neighbour tile  SetMoverPath(moverIndex, tileCoord);??
        }
        //  Debug.Log("Selected Mover index "+ SelectedMoverIndex);
    }



    public void SetMoverPath(int moverIndex, int2 tagretCoordinate)
    {

        //Profiler.BeginSample("SetMoverPath");
        //int2 startCoord = ChunkUtility.GetEntityComponentValueAtIndex<CoordinateComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex).Coordinate;
        //SoldierComponent soldierComponent = ChunkUtility.GetEntityComponentValueAtIndex<SoldierComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex);

        //NativeArray<int2> path = GetMoverPath.Invoke(moverIndex, startCoord, tagretCoordinate);
        //if (path.Length == 0)
        //{ ResetSelectedMoverIndex(); return; }

        //ChunkUtility.GetEntityComponentValueAtIndex<MoverComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex).Path = path;
        //ChunkUtility.GetEntityComponentValueAtIndex<MoverComponent>(_world.ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)][0], moverIndex).HasPath = true;

        //ResetSelectedMoverIndex();
        //Profiler.EndSample();
    }

    public void SetSelectedMover(int selectedMoverIndex)
    {
        SelectedMoverIndex = selectedMoverIndex;
    }
    public int GetSelectedMoverIndex()
    {
        return SelectedMoverIndex;
    }

    public void ResetSelectedMoverIndex()
    {
        SelectedMoverIndex = -1;
    }


}
