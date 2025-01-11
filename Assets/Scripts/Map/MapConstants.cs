using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MapConstants
{
    public static readonly Dictionary<TerrainType, float2> TerrainOffsets = new()
    {
        { TerrainType.None, new float2(-1, -1) },
        { TerrainType.LightGreen, new float2(0, 0.5f) },
        { TerrainType.DarkGreen, new float2(0.25f, 0.5f) }
    };

    public static readonly Dictionary<BuildingType, float2> BuildingOffsets = new()
    {
        { BuildingType.None, new float2(-1, -1) },
        { BuildingType.Barrack, new float2(0.5f, 0.5f) },
        { BuildingType.PowerPlant, new float2(0.75f, 0.5f) },
        { BuildingType.PreviewRed, new float2(0, 0) },
    };

    public static readonly Dictionary<SoldierType, float2> SoldierOffsets = new()
    {
        { SoldierType.None, new float2(-1, -1) },
        { SoldierType.Soldier1, new float2(0.25f, 0) },
        { SoldierType.Soldier2, new float2(0.5f, 0) },
        { SoldierType.Soldier3, new float2(0.75f, 0) },
    };

    public static readonly int SoldierHealth = 10;

    public static readonly Dictionary<SoldierType, int> SoldierDamages = new()
    {
        { SoldierType.None, -1 },
        { SoldierType.Soldier1, 10 },
        { SoldierType.Soldier2, 5 },
        { SoldierType.Soldier3, 2 },
    };

    public static readonly Dictionary<BuildingType, int> BuildingHealth = new()
    {
        { BuildingType.None, -1 },
        { BuildingType.Barrack, 100 },
        { BuildingType.PowerPlant, 50 },
    };
}
