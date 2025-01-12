using Game.ECS.Base.Systems;
using Game.ECS.Base;
using Game.Factory;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;
using Game.ECS.Base.Components;


namespace Game.ECS.Systems
{
    public class TileCreationSystem : IInitSystem
    {

        private FactoryManager _factoryManager;

        public TileCreationSystem(FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
        }
        public void Init(SystemManager systemManager)
        {
            CreateTiles(systemManager.GetWorld());
        }

        public void CreateTiles(ECSWorld eCSWorld)
        {
            int chunkXSize = MapSettings.TileChunkEdgeSize;
            int chunkYSize = MapSettings.TileChunkEdgeSize;

            int xChunkNumber = (MapSettings.MapWidth / chunkXSize);
            int yChunkNumber = (MapSettings.MapHeight / chunkYSize);

            for (int yCN = 0; yCN < yChunkNumber; yCN++)
            {
                for (int xCN = 0; xCN < xChunkNumber; xCN++)
                {
                    for (int y = 0; y < chunkYSize; y++)
                    {
                        for (int x = 0; x < chunkXSize; x++)
                        {
                            int absoluteX = x + xCN * chunkXSize;
                            int absoluteY = y + yCN * chunkYSize;
                            int2 coordinate = new int2(absoluteX, absoluteY);
                            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(absoluteX, absoluteY, 0),Quaternion.identity,Vector3.one);

                            if (absoluteX >= MapSettings.MapWidth || absoluteY >= MapSettings.MapHeight)
                                continue;

                            int newEntityID=eCSWorld.CreateNewEntity();

                            eCSWorld.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                                               _factoryManager.GetInstance<CoordinateComponent>(coordinate));
                            eCSWorld.AddComponentToEntity<TileComponent>(newEntityID,
                                                                         _factoryManager.GetInstance<TileComponent>( -1 ));
                            eCSWorld.AddComponentToEntity<RenderComponent>(newEntityID,
                                                                          _factoryManager.GetInstance<RenderComponent>(new object[2] { matrix, ((absoluteX + absoluteY) % 2 == 0) ? MapConstants.TerrainOffsets[TerrainType.LightGreen] : MapConstants.TerrainOffsets[TerrainType.DarkGreen]}));
                            eCSWorld.AddComponentToEntity<QuadTreeLeafComponent>(newEntityID,
                                                                                 _factoryManager.GetInstance<QuadTreeLeafComponent>(newEntityID,new Rect(coordinate.x-0.5f,
                                                                                                                                                         coordinate.y - 0.5f, 1,1)));
                        }
                    }
                }
            }
        }
    }

}
