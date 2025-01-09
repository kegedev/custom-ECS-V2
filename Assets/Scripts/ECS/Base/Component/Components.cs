using UnityEngine;
using Unity.Mathematics;
using System.ComponentModel;


namespace Game.ECS.Base.Components
{
    public struct CoordinateComponent
    {
        public int2 Coordinate;
    }

    public struct TileComponent
    {
        public int MoverIndex;
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
}

