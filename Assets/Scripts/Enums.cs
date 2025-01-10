using UnityEngine;

public enum ComponentMask : ushort
{
    CoordinateComponent = 1 << 0,
    TileComponent = 1 << 1,
    RenderComponent = 1 << 2,
    QuadTreeLeafComponent = 1 << 3,
    MoverComponent = 1 << 4,
    AreaComponent= 1 << 5,
    PlaceHolderComponent= 1 << 6,
    //RotationComponent = 1 << 1,
    //ScaleComponent = 1 << 2,
    //StaticRenderComponent = 1 << 4,
    //SoldierComponent = 1 << 6,
    //DynamicRenderComponent = 1 << 9,
}
