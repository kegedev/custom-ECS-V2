using Game.ECS.Base.Components;
using Game.ECS.Base;
using Game.ECS.Base.Systems;
using Game.Factory;
using System;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.LightTransport;


namespace Game.ECS.Systems
{
    public class BuildingCreationSystem : IInitSystem
    {
        ECSWorld world;
        private FactoryManager _factoryManager;
        public Action<CoordinateComponent, int> SetOccupant;
        public Func<CoordinateComponent, int> GetOccupant;

        public BuildingCreationSystem(FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
        }
        public void Init(SystemManager systemManager)
        {
            world = systemManager.GetWorld();

           // CreateBuildings(eCSWorld);
        }

        public void CreateBuilding(BuildingType buildingType, int rootTileId)
        {
         
            int newEntityID = world.CreateNewEntity();
            var renderComponentContainer = world.GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
            var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);

            var rootTileCoord = coordinateComponentContainer.GetComponent(rootTileId);

           
            CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(rootTileCoord.Coordinate);

            world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                               ComponentMask.CoordinateComponent,
            coordinateComponent);

            world.AddComponentToEntity<AreaComponent>(newEntityID,
                                                          ComponentMask.AreaComponent,
                                                          _factoryManager.GetInstance<AreaComponent>(new object[] { 5, 5 }));

            //Placeholder creation for building
            for (int w = 0; w < 5; w++)
            {
                for (int h = 0; h < 5; h++)
                {
                    int pcAbsoluteX = rootTileCoord.Coordinate.x + w;
                    int pcAbsoluteY = rootTileCoord.Coordinate.y + h;

                    if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                        continue;
                    int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(ComponentMask.QuadTreeLeafComponent),
                    world.quadTreeNodeDatas,
                    world.QuadtreeNodeIndexes,
                                                   world.QuadtreeLeafIndexes,
                    world.TileQuadtreeRoot,
                                                   new Vector2(pcAbsoluteX, pcAbsoluteY));
                    var renderComp = renderComponentContainer.GetComponent(tileId);
                    var coordinateComp = coordinateComponentContainer.GetComponent(tileId);
                    
                    renderComp.TextureOffset = MapConstants.BuildingOffsets[buildingType];
                    renderComponentContainer.UpdateComponent(tileId, renderComp);
                    SetOccupant.Invoke(coordinateComp, newEntityID);
                }
            }
        }

        public void CreateBuildings(ECSWorld world)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int absoluteX = UnityEngine.Random.Range(50, 120);
                    int absoluteY = UnityEngine.Random.Range(50, 120);
                    int2 coordinate = new int2(absoluteX, absoluteY);
                    Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(absoluteX, absoluteY, 0), Quaternion.identity, Vector3.one * 0.95f);

                    if (absoluteX >= MapSettings.MapWidth || absoluteY >= MapSettings.MapHeight)
                        continue;

                    int newEntityID = world.CreateNewEntity();
                 
                    CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(coordinate);

                    world.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                                       ComponentMask.CoordinateComponent,
                                                                       coordinateComponent);

                    world.AddComponentToEntity<AreaComponent>(newEntityID,
                                                                  ComponentMask.AreaComponent,
                                                                  _factoryManager.GetInstance<AreaComponent>(new object[] { 5,5}));

                    var renderComponentContainer = world.GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
                    var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
                    //Placeholder creation for building
                    for (int w = 0; w < 5; w++)
                    {
                        for (int h = 0; h < 5; h++)
                        {
                            int pcAbsoluteX = coordinate.x+w;
                            int pcAbsoluteY = coordinate.y + h;

                            if (pcAbsoluteX >= MapSettings.MapWidth || pcAbsoluteY >= MapSettings.MapHeight)
                                continue;
                            int tileId = QuerySystem.GetEntityId(world.GetComponentContainer<QuadTreeLeafComponent>(ComponentMask.QuadTreeLeafComponent),
                                                           world.quadTreeNodeDatas,
                                                           world.QuadtreeNodeIndexes,
                                                           world.QuadtreeLeafIndexes,
                                                           world.TileQuadtreeRoot,
                                                           new Vector2(pcAbsoluteX,pcAbsoluteY));
                            var renderComp= renderComponentContainer.GetComponent(tileId);
                            var coordinateComp = coordinateComponentContainer.GetComponent(tileId);
                          
                            renderComp.TextureOffset = new float2(0.5f,0.5f);
                            renderComponentContainer.UpdateComponent(tileId,renderComp);
                            SetOccupant.Invoke(coordinateComp, newEntityID);
                        }
                    }

                 

                }
            }
        }
    }
}
