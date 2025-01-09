using UnityEngine;
using Unity.Mathematics;
using System.ComponentModel;
using Unity.Collections;


namespace Game.ECS.Base.Components
{
    public struct CoordinateComponent
    {
        public int2 Coordinate;
    }

    public struct TileComponent
    {
        public int OccupantEntityID;
    }
    public struct RenderComponent
    {
        public Matrix4x4 TRS;
        public float2 TextureOffset;
    }

    public struct QuadTreeLeafComponent
    {
        public int LeafID;
        public Rect Rect;
    }

    public struct MoverComponent
    {
        public NativeArray<int2> Path;//buradan kaldırılacak (?)
    }
}

