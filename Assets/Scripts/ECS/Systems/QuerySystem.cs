using Game.ECS.Base;
using Game.ECS.Base.Components;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

internal static class QuerySystem
{
    //TODO: GetEntityId with coordinate
    internal static int GetEntityId(ComponentContainer<QuadTreeLeafComponent> componentContainer,
                                   in QuadTreeData quadTreeData,
                                   Vector2 point)
    {
        if (!quadTreeData.TileQuadtreeRoot.Rect.Contains(point))
        {
            Debug.LogError("OUT OF MAP " + point);
            return -1;
        }

        QuadTreeNodeData currentNode = quadTreeData.TileQuadtreeRoot;
        while (currentNode.IsDivided)
        {
            bool foundInChild = false;
            for (int i = 0; i < 4; i++)
            {
                if (quadTreeData.QuadTreeNodeDatas[quadTreeData.QuadtreeNodeIndexes[currentNode.NodesStart + i]].Rect.Contains(point))
                {
                    currentNode = quadTreeData.QuadTreeNodeDatas[quadTreeData.QuadtreeNodeIndexes[currentNode.NodesStart + i]];
                    foundInChild = true;
                    break;
                }
            }
            if (!foundInChild)
            {
                Debug.LogError("Point not found in any child nodes.");
                return -1;
            }
        }




        for (int i = 0; i < currentNode.Capacity; i++)
        {
            QuadTreeLeafComponent quadTreeLeafComponent = componentContainer.GetComponent(quadTreeData.QuadtreeLeafIndexes[currentNode.LeavesStart + i]);

            if (quadTreeLeafComponent.Rect.Contains(point))
            {

                return quadTreeData.QuadtreeLeafIndexes[currentNode.LeavesStart + i];
            }
        }

        Debug.LogError("Leaf containing the point not found.");
        return -1;
    }

    public static int GetClosestUnoccupiedNeighbour(int2 coordinate, ECSWorld world)
    {
        List<int2> neighbors = new List<int2>();

        foreach (var direction in MapSettings.Directions)
        {
            Vector2 checkCoordinate = new Vector2(coordinate.x + direction.x, coordinate.y + direction.y);

            if (checkCoordinate.x >= 0 && checkCoordinate.x < MapSettings.MapWidth && checkCoordinate.y >= 0 && checkCoordinate.y < MapSettings.MapHeight)
            {
                int tileId = GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                     world.QuadTreeData,
                                                     checkCoordinate);

                var coordinateComp = world.GetComponent<CoordinateComponent>(tileId);
                var tileComp = world.GetComponent<TileComponent>(tileId);

                if (tileComp.OccupantEntityID == -1)
                {
                    neighbors.Add(coordinateComp.Coordinate);
                }
            }
        }
        int2 cn = FindClosestCoordinate(coordinate, neighbors);
        return GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                             world.QuadTreeData,
                                                             new float2(cn.x, cn.y));

    }

    public static int GetClosestUnoccupiedNeighbourOfArea(ECSWorld world, int2 coordinate, int width, int height)
    {

        List<int2> neighbors = new List<int2>();

        Vector2 checkCoordinate = new Vector2(coordinate.x, coordinate.y);

        int tileId = GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                                 world.QuadTreeData,
                                                                 checkCoordinate);

        int buildingEntityId = world.GetComponent<TileComponent>(tileId).OccupantEntityID;

        int2 buildingRoot = world.GetComponent<CoordinateComponent>(buildingEntityId).Coordinate;

        for (int x = -1; x < width + 1; x++)
        {
            checkCoordinate = new Vector2(coordinate.x + x, coordinate.y - 1);
            int2 coordinateOfNeigh = CheckTileAndGetCoordinate(world, checkCoordinate);
            if (coordinateOfNeigh.x != -1) neighbors.Add(coordinateOfNeigh);
        }
        for (int x = -1; x < width + 1; x++)
        {
            checkCoordinate = new Vector2(buildingRoot.x + x, buildingRoot.y + height);
            int2 coordinateOfNeigh = CheckTileAndGetCoordinate(world, checkCoordinate);
            if (coordinateOfNeigh.x != -1) neighbors.Add(coordinateOfNeigh);
        }
        for (int y = -1; y < height + 1; y++)
        {
            checkCoordinate = new Vector2(buildingRoot.x - 1, buildingRoot.y + y);
            int2 coordinateOfNeigh = CheckTileAndGetCoordinate(world, checkCoordinate);
            if (coordinateOfNeigh.x != -1) neighbors.Add(coordinateOfNeigh);
        }
        for (int y = -1; y < height + 1; y++)
        {
            checkCoordinate = new Vector2(buildingRoot.x + width, buildingRoot.y + y);
            int2 coordinateOfNeigh = CheckTileAndGetCoordinate(world, checkCoordinate);
            if (coordinateOfNeigh.x != -1) neighbors.Add(coordinateOfNeigh);
        }

        if (neighbors == null || neighbors.Count == 0) Debug.LogError("No Empty Neighbour of Building");
        int2 cn = FindClosestCoordinate(coordinate, neighbors);

        return GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                             world.QuadTreeData,
                                                             new float2(cn.x, cn.y));

    }

    public static int2 FindClosestCoordinate(int2 target, List<int2> int2List)
    {
        if (int2List == null || int2List.Count == 0)
        {
            throw new Exception("The int2 list cannot be null or empty.");
        }

        int2 closest = int2List[0];
        int minDistanceSquared = CalculateDistanceSquared(target, closest);

        foreach (var point in int2List)
        {
            int distanceSquared = CalculateDistanceSquared(target, point);
            if (distanceSquared < minDistanceSquared)
            {
                closest = point;
                minDistanceSquared = distanceSquared;
            }
        }

        return closest;
    }

    private static int CalculateDistanceSquared(int2 a, int2 b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return dx * dx + dy * dy;
    }

    private static int2 CheckTileAndGetCoordinate(ECSWorld world, float2 checkCoordinate)
    {
        if (checkCoordinate.x >= 0 && checkCoordinate.x < MapSettings.MapWidth && checkCoordinate.y >= 0 && checkCoordinate.y < MapSettings.MapHeight)
        {
            int tileId = GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                        world.QuadTreeData,
                                                        checkCoordinate);

            var coordinateComp = world.GetComponent<CoordinateComponent>(tileId);
            var tileComp = world.GetComponent<TileComponent>(tileId);

            if (tileComp.OccupantEntityID == -1)
            {
                return coordinateComp.Coordinate;
            }
            else return new int2(-1, -1);
        }
        else return new int2(-1, -1);
    }
}
