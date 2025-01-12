using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.ECS.Base;
using UnityEngine;
using System;

namespace Game.ECS.Systems
{
    public class OccupancySystem
    {

        private ECSWorld _world;
        public OccupancySystem(ECSWorld world) 
        {
                _world = world;
        }

        public void SetTileOccupant(CoordinateComponent coordinateComponent, int occupantEntityId)
        {
            Vector2 pos = new Vector2(coordinateComponent.Coordinate.x, coordinateComponent.Coordinate.y);

            int tileEntityId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),_world.QuadTreeData,pos);

            ref var tileComponent=ref _world.GetComponent<TileComponent>(tileEntityId);
            tileComponent.OccupantEntityID = occupantEntityId;

        }

        public int GetTileOccupant(CoordinateComponent coordinateComponent)
        {
            Vector2 pos = new Vector2(coordinateComponent.Coordinate.x, coordinateComponent.Coordinate.y);

            int tileEntityId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),_world.QuadTreeData,pos);

            ref var tileComponent = ref _world.GetComponent<TileComponent>(tileEntityId);
            return tileComponent.OccupantEntityID;

        }


    }
}
