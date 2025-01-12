using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using System.Drawing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;

namespace Game.ECS.Systems
{
    public class MovementSystem : IUpdateSystem
    {
        ECSWorld _world;
        private float movementTimer = 0f;
        private const float movementInterval = 0.05f;
        public Action<CoordinateComponent, int> SetTileOccupant;
        public Func<int, int2, int2, NativeArray<int2>> GetMoverPath;
        public ushort ActiveStateMask => (ushort)GameState.MainState;

        public MovementSystem(ECSWorld world)
        {
            _world=world;
        }

        public void Update(SystemManager systemManager)
        {
            if (!systemManager.GetWorld().HasComponentContainer<MoverComponent>()) return;
            movementTimer += Time.deltaTime;
            if (movementTimer >= movementInterval)
            {
                movementTimer -= movementInterval;

                MoveEntities(systemManager.GetWorld());
            }

        }

        private void MoveEntities(ECSWorld world)
        {
            var moverComponentContainer = world.GetComponentContainer<MoverComponent>();
            var coordinateComponentContainer = world.GetComponentContainer<CoordinateComponent>();
            var renderComponentContainer = world.GetComponentContainer<RenderComponent>();

            for (int i = 0; i < moverComponentContainer.EntityCount; i++)
            {

                    int entityId = moverComponentContainer.EntityIds[i];
                    ref var moverComponent = ref moverComponentContainer.GetComponent(entityId);
                    
                    if (!moverComponent.HasPath) continue;
                    ref var coordinateComponent = ref coordinateComponentContainer.GetComponent(entityId);
                    ref var renderComponent = ref renderComponentContainer.GetComponent(entityId);

                    if (moverComponent.PathStepNumber != moverComponent.Path.Length)
                    {
                        SetTileOccupant.Invoke(coordinateComponent, -1);
                        coordinateComponent.Coordinate = moverComponent.Path[moverComponent.PathStepNumber];
                        SetTileOccupant.Invoke(coordinateComponent, entityId);

                        renderComponent.TRS = Matrix4x4.TRS(new Vector3(moverComponent.Path[moverComponent.PathStepNumber].x,moverComponent.Path[moverComponent.PathStepNumber].y,0),//* _mapSettings.TileEdgeSize
                                                            Quaternion.identity,
                                                            Vector3.one);
                        
                        moverComponent.PathStepNumber++;

                    }
                    else
                    {
                    moverComponent.HasPath = false;
                    moverComponent.Path.Dispose();
                    moverComponent.PathStepNumber = 0;

                    }
                }
            
        }

        public void SetMoverPath(int targetTileId,int selectedMoverID)
        {
            ref var moverComponent = ref _world.GetComponent<MoverComponent>(selectedMoverID);
            moverComponent.HasPath = false;
            moverComponent.PathStepNumber = 0;
            int2 startCoord = _world.GetComponent<CoordinateComponent>(selectedMoverID).Coordinate;
            int2 targetCoord = _world.GetComponent<CoordinateComponent>(targetTileId).Coordinate;

            NativeArray<int2> path = GetMoverPath.Invoke(selectedMoverID, startCoord, targetCoord);
            moverComponent.Path = path;
            moverComponent.HasPath = true;

        }
        public void UpdateGameState(GameState gameState)
        {
            if (!_world.HasComponentContainer<MoverComponent>()) return;
            var moverComponentContainer = _world.GetComponentContainer<MoverComponent>();
            for (int i = 0; i < moverComponentContainer.EntityCount; i++)
            {

                int entityId = moverComponentContainer.EntityIds[i];
                ref var moverComponent = ref moverComponentContainer.GetComponent(entityId);

                if (!moverComponent.HasPath) continue;
                int tileId = QuerySystem.GetEntityId(_world.GetComponentContainer<QuadTreeLeafComponent>(),
                                                   _world.QuadTreeData,
                                                   new Vector2(moverComponent.Path[moverComponent.Path.Length - 1].x, moverComponent.Path[moverComponent.Path.Length - 1].y));
                SetMoverPath(tileId, entityId);
            }
        }
    }
}
