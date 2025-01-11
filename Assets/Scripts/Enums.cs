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

public enum TerrainType : ushort
{
    None = 0,
    LightGreen =1,
    DarkGreen =2,

}

public enum BuildingType : ushort
{
    None=0,
    Barrack=1,
    PowerPlant=2,
    PreviewRed=3,
}

public enum SoldierType:ushort
{
    None = 0,
    Soldier1 =1,
    Soldier2=2,
    Soldier3=3
}

public enum GameState:ushort
{
    MainState=1<<0,
    Construction = 1 << 1,
}
