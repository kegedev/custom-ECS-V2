using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class MapSettings
{
    public static int MapWidth { get; private set; }
    public static int MapHeight { get; private set; }
    public static float TileEdgeSize { get; private set; }
    public static int TileChunkEdgeSize { get; private set; }
    public static NativeArray<int2> Directions { get; private set; }

    public static void Initialize(int mapWidth, int mapHeight, float tileEdgeSize, int tileChunkEdgeSize)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        TileEdgeSize = tileEdgeSize;
        TileChunkEdgeSize = tileChunkEdgeSize;

        Directions = new NativeArray<int2>(8, Allocator.Persistent)
        {
            [0] = new int2(0, 1),    // Up
            [1] = new int2(1, 1),    // Up-Right
            [2] = new int2(1, 0),    // Right
            [3] = new int2(1, -1),   // Down-Right
            [4] = new int2(0, -1),   // Down
            [5] = new int2(-1, -1),  // Down-Left
            [6] = new int2(-1, 0),   // Left
            [7] = new int2(-1, 1)    // Up-Left
        };
    }

    public static void Dispose()
    {
        if (Directions.IsCreated)
        {
            Directions.Dispose();
        }
    }
}