using Game.ECS.Base.Components;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

internal static class QuerySystem
{

    internal static int GetEntityId(ComponentContainer<QuadTreeLeafComponent> componentContainer,
                                   in NativeList<QuadTreeNodeData> quadTreeNodeDatas,
                                   in NativeList<int> QuadtreeNodeIndexes,
                                   in NativeList<int> QuadtreeLeavesIndexes,
                                   in QuadTreeNodeData rootNode,
                                   Vector2 point)
    {
        if (!rootNode.Rect.Contains(point))
        {
            Debug.LogError("Point is out of bounds of the root node. "+ point);
            return -1;
        }

        QuadTreeNodeData currentNode = rootNode;
        while (currentNode.IsDivided)
        {
            bool foundInChild = false;
            for (int i = 0; i < 4; i++)
            {
                if (quadTreeNodeDatas[QuadtreeNodeIndexes[currentNode.NodesStart + i]].Rect.Contains(point))
                {
                    currentNode = quadTreeNodeDatas[QuadtreeNodeIndexes[currentNode.NodesStart + i]];
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
            QuadTreeLeafComponent quadTreeLeafComponent= componentContainer.GetComponent(QuadtreeLeavesIndexes[currentNode.LeavesStart + i]);
           
            if (quadTreeLeafComponent.Rect.Contains(point))
            {
                Debug.Log(QuadtreeLeavesIndexes[currentNode.LeavesStart + i]);
                return QuadtreeLeavesIndexes[currentNode.LeavesStart + i];
            }
        }

        Debug.LogError("Leaf containing the point not found.");
        return -1;
    }

 
}
