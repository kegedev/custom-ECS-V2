using UnityEngine;

public enum ComponentMask : ushort
{
    CoordinateComponent = 1 << 0,
    TileComponent = 1 << 1,
    RenderComponent = 1 << 2,
    //RotationComponent = 1 << 1,
    //ScaleComponent = 1 << 2,
    //StaticRenderComponent = 1 << 4,
    //QuadTreeLeafComponent = 1 << 5,
    //SoldierComponent = 1 << 6,
    //MoverComponent = 1 << 7,
    //DynamicRenderComponent = 1 << 9,
}
