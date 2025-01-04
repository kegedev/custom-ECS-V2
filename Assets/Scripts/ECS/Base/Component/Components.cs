using UnityEngine;
using Unity.Mathematics;


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
    }
}

