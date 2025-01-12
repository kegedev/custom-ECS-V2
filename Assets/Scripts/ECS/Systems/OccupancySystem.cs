using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.ECS.Base;
using UnityEngine;
using System;

namespace Game.ECS.Systems
{
    public class OccupancySystem : IInitSystem
    {

        private ECSWorld _world;

        public void Init(SystemManager systemManager)
        {
            _world = systemManager.GetWorld();
        }

        public void SetTileOccupant(CoordinateComponent coordinateComponent, int occupantEntityId)
        {
            Vector2 pos = new Vector2(coordinateComponent.Coordinate.x, coordinateComponent.Coordinate.y);

            int tileEntityId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)_world.ComponentContainers[typeof(QuadTreeLeafComponent)],
                                       _world.QuadTreeData,
                                       pos);

            ComponentContainer<TileComponent> componentContainer = (ComponentContainer<TileComponent>)_world.ComponentContainers[typeof(TileComponent)];
            TileComponent tileComponent = componentContainer.GetComponent(tileEntityId);
            tileComponent.OccupantEntityID = occupantEntityId;
            componentContainer.Components[tileEntityId] = tileComponent;
            _world.ComponentContainers[typeof(TileComponent)] = componentContainer;


        }

        public int GetTileOccupant(CoordinateComponent coordinateComponent)
        {
            Vector2 pos = new Vector2(coordinateComponent.Coordinate.x, coordinateComponent.Coordinate.y);

            int tileEntityId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)_world.ComponentContainers[typeof(QuadTreeLeafComponent)],
                                       _world.QuadTreeData,
                                       pos);

            ComponentContainer<TileComponent> componentContainer = (ComponentContainer<TileComponent>)_world.ComponentContainers[typeof(TileComponent)];
            TileComponent tileComponent = componentContainer.GetComponent(tileEntityId);
            return tileComponent.OccupantEntityID;

        }


    }
}
