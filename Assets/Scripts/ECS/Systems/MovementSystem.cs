using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using System;
using System.Drawing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.Systems
{
    public class MovementSystem : IUpdateSystem
    {
        private float movementTimer = 0f;
        private const float movementInterval = 0.05f;
        public Action<CoordinateComponent, int> SetTileOccupant;
        public ushort ActiveStateMask => (ushort)GameState.MainState;

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

        internal void MoveEntities(ECSWorld world)
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
                       // SetTileOccupant.Invoke(coordinateComponent, entityId);

                    }
                }
            
        }
    }
}
