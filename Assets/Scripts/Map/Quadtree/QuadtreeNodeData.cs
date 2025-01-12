using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct QuadTreeNodeData
{
    public int LeafCount;
    public Rect Rect;
    public int Capacity;
    public bool IsDivided;
    public int LeavesStart;
    public int NodesStart;

}

public struct QuadTreeData
{
    public NativeList<QuadTreeNodeData> QuadTreeNodeDatas;
    public NativeList<int> QuadtreeNodeIndexes;
    public NativeList<int> QuadtreeLeafIndexes;
    public int QuadtreeNodeIndex;
    public QuadTreeNodeData TileQuadtreeRoot;
}









