using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.Systems
{
    public class AStarSystem
    {
        ECSWorld _world;

        int maxSteps = 2000;

        public AStarSystem(ECSWorld world)
        {
            _world=world;
        }

        public NativeArray<int2> GetMoverPath(int moverIndex, int2 startcoord, int2 tagretCoordinate)
        {
            AStarNode startNode = new AStarNode()
            {
                MoverIndex = moverIndex,
                Coordinate = startcoord,
            };
            AStarNode targetNode = new AStarNode()
            {
                MoverIndex = -1,
                Coordinate = tagretCoordinate
            };
            List<AStarNode> aStarNodes = FindPath(startNode, targetNode, _world.QuadTreeData.TileQuadtreeRoot, MapSettings.Directions, MapSettings.MapWidth, MapSettings.MapHeight);

            if (aStarNodes == null)
            {
                Debug.Log("astarNodes null");
                return new NativeArray<int2>(0, Allocator.Persistent);
            }
            NativeArray<int2> path = new NativeArray<int2>(aStarNodes.Count, Allocator.Persistent);
            for (int i = 0; i < aStarNodes.Count; i++)
            {
                path[i] = aStarNodes[i].Coordinate;
            }
            return path;
        }


        internal List<AStarNode> FindPath(AStarNode startTile, AStarNode goalTile, QuadTreeNodeData quadtreeNode, NativeArray<int2> directions, int mapWidth, int mapHeight)
        {
            int nodeIdCounter = 0;
            NativeList<AStarNode> allNodes = new NativeList<AStarNode>(Allocator.Temp);
            NativeList<AStarNode> openList = new NativeList<AStarNode>(Allocator.Temp);
            NativeList<AStarNode> closedList = new NativeList<AStarNode>(Allocator.Temp);

            startTile.NodeID = nodeIdCounter++;
            goalTile.NodeID = nodeIdCounter++;
            allNodes.Add(startTile);
            allNodes.Add(goalTile);

            openList.Add(startTile);
            int stepCount = 0;
            while (openList.Length > 0)
            {
                if (stepCount++ > maxSteps)
                {
                    stepCount = 0;
                    Debug.LogError("Pathfinding aborted: Exceeded maximum steps.");
                    return null;
                }

                AStarNode currentTile = openList[0];

                for (int i = 1; i < openList.Length; i++)
                {
                    if (openList[i].fCost < currentTile.fCost ||
                        (openList[i].fCost == currentTile.fCost && openList[i].hCost < currentTile.hCost))
                    {
                        currentTile = openList[i];
                    }
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i].NodeID == currentTile.NodeID)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentTile);

                if (currentTile.Coordinate.x == goalTile.Coordinate.x && currentTile.Coordinate.y == goalTile.Coordinate.y)
                {
                    goalTile = currentTile;
                    return RetracePath(allNodes, startTile, goalTile);
                }

                var neighbors = GetNeighbors(currentTile);


                for (int i = 0; i < neighbors.Count; i++)
                {
                    AStarNode aStarNode = neighbors[i];
                    aStarNode.ParentId = GetIndexFromId(allNodes, currentTile.NodeID);
                    aStarNode.NodeID = nodeIdCounter++;
                    neighbors[i] = aStarNode;
                    allNodes.Add(neighbors[i]);
                }

                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (neighbors[i].MoverIndex != -1 || ListContainsNode(closedList, neighbors[i]))
                        continue;

                    float newMovementCostToNeighbor = currentTile.gCost + GetDistance(currentTile, neighbors[i]);
                    if (newMovementCostToNeighbor < neighbors[i].gCost || !ListContainsNode(openList, neighbors[i]))
                    {
                        AStarNode tempAStarNode = neighbors[i];
                        tempAStarNode.gCost = newMovementCostToNeighbor;
                        tempAStarNode.hCost = GetDistance(neighbors[i], goalTile);
                        tempAStarNode.ParentId = currentTile.NodeID;
                        neighbors[i] = tempAStarNode;

                        if (!ListContainsNode(openList, neighbors[i]))
                        {
                            openList.Add(neighbors[i]);
                        }
                    }
                }

            }
            return null;
        }

        private List<AStarNode> RetracePath(in NativeList<AStarNode> aStarNodes, AStarNode startTile, AStarNode goalTile)
        {
            List<AStarNode> path = new List<AStarNode>();
            AStarNode currentTile = goalTile;
            while (currentTile.Coordinate.x != startTile.Coordinate.x || currentTile.Coordinate.y != startTile.Coordinate.y)
            {
                path.Add(currentTile);
                currentTile = aStarNodes[GetIndexFromId(in aStarNodes, currentTile.ParentId)];
            }
            path.Add(startTile);
            path.Reverse();
            return path;
        }

        private List<AStarNode> GetNeighbors(AStarNode CurrentNode)
        {
            List<AStarNode> neighbors = new List<AStarNode>();

            foreach (var direction in MapSettings.Directions)
            {
                Vector2 checkCoordinate = new Vector2(CurrentNode.Coordinate.x + direction.x, CurrentNode.Coordinate.y + direction.y);

                if (checkCoordinate.x >= 0 && checkCoordinate.x < MapSettings.MapWidth && checkCoordinate.y >= 0 && checkCoordinate.y < MapSettings.MapHeight)
                {
                    int tileId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                       _world.QuadTreeData,
                                                       checkCoordinate);

                    var coordinateComp = _world.GetComponent<CoordinateComponent>(tileId);
                    var tileComp = _world.GetComponent<TileComponent>(tileId);

                    neighbors.Add(new AStarNode()
                    {
                        MoverIndex = tileComp.OccupantEntityID,
                        Coordinate = coordinateComp.Coordinate
                    });



                }



            }



            return neighbors;
        }
        //[BurstCompile]
        //private struct GetNeighboursJob : IJobParallelFor
        //{
        //    public AStarNode CurrentNode;
        //    [WriteOnly] public NativeArray<AStarNode> Neighbours;
        //    [ReadOnly] public NativeArray<int2> Directions;
        //    public int MapWidth;
        //    public int MapHeight;
        //    [NativeDisableUnsafePtrRestriction]
        //    [ReadOnly] public ComponentContainer<QuadTreeLeafComponent> LeafComponentContainer;
        //    [ReadOnly] public ComponentContainer<CoordinateComponent> CoordinateComponentContainer;
        //    [ReadOnly] public ComponentContainer<TileComponent> TileComponentContainer;
        //    [NativeDisableUnsafePtrRestriction]
        //    [ReadOnly] public NativeList<QuadTreeNodeData> QuadTreeNodeDatas;
        //    [ReadOnly] public NativeList<int> QuadTreeNodeIndexes;
        //    [ReadOnly] public NativeList<int> QuadTreeLeafIndexes;
        //    [ReadOnly] public QuadTreeNodeData TileQuadtreeRoot;
        //    public void Execute(int index)
        //    {
        //        Vector2 checkCoordinate = new Vector2(CurrentNode.Coordinate.x + Directions[index].x, CurrentNode.Coordinate.y + Directions[index].y);
        //        if (checkCoordinate.x >= 0 && checkCoordinate.x < MapWidth && checkCoordinate.y >= 0 && checkCoordinate.y < MapHeight)
        //        {
        //            int tileId=QuerySystem.GetEntityId(LeafComponentContainer,
        //                                               QuadTreeNodeDatas,
        //                                               QuadTreeNodeIndexes,
        //                                               QuadTreeLeafIndexes,
        //                                               TileQuadtreeRoot,
        //                                               checkCoordinate);

        //            var coordinateComp = CoordinateComponentContainer.GetComponent(tileId);
        //            var tileComp = TileComponentContainer.GetComponent(tileId);
        //            Neighbours[index] = new AStarNode()
        //            {
        //                MoverIndex = tileComp.OccupantEntityID,
        //                Coordinate = coordinateComp.Coordinate
        //            };



        //        }

        //    }
        //}

        private float GetDistance(AStarNode a, AStarNode b)
        {
            float dstX = Mathf.Abs(a.Coordinate.x - b.Coordinate.x);
            float dstY = Mathf.Abs(a.Coordinate.y - b.Coordinate.y);

            return dstX + dstY;
        }

        internal int GetIndexFromId(in NativeList<AStarNode> allnodes, int id)
        {
            for (int i = 0; i < allnodes.Length; i++)
            {
                if (allnodes[i].NodeID == id) return i;
            }
            return -1;
        }

        private bool ListContainsNode(NativeList<AStarNode> list, AStarNode node)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].NodeID == node.NodeID)
                    return true;
            }
            return false;
        }

        internal static void CopyAStarNode(AStarNode source, ref AStarNode target)
        {
            target.NodeID = source.NodeID;
            target.Coordinate = source.Coordinate;
            target.MoverIndex = source.MoverIndex;
            target.ParentId = source.ParentId;
            target.gCost = source.gCost;
            target.hCost = source.hCost;
        }
    }

    internal struct AStarNode
    {
        internal int NodeID;
        internal int2 Coordinate;
        internal int MoverIndex;
        internal int ParentId;
        internal float gCost;
        internal float hCost;
        internal float fCost => gCost + hCost;
    }

}

