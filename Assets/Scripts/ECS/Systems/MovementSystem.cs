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

        //public Action<ComponentMask, int, int2> UpdateRenderMatrix;


        public void Update(SystemManager systemManager)
        {
            movementTimer += Time.deltaTime;
            if (movementTimer >= movementInterval)
            {
                movementTimer -= movementInterval;

                var moverComponentContainer = systemManager.GetWorld().GetComponentContainer<MoverComponent>(ComponentMask.MoverComponent);
                var coordinateComponentContainer = systemManager.GetWorld().GetComponentContainer<CoordinateComponent>(ComponentMask.CoordinateComponent);
                var renderComponentContainer = systemManager.GetWorld().GetComponentContainer<RenderComponent>(ComponentMask.RenderComponent);
                
                MoveEntities(moverComponentContainer, coordinateComponentContainer,renderComponentContainer);
               // systemManager.GetWorld().ChunkContainers[(ushort)(ComponentMask.CoordinateComponent | ComponentMask.SoldierComponent | ComponentMask.MoverComponent)] = tempChunks;
            }

        }

        internal void MoveEntities(ComponentContainer<MoverComponent> moverComponentContainer, 
                                   ComponentContainer<CoordinateComponent> coordinateComponentContainer,
                                   ComponentContainer<RenderComponent> renderComponetContainer)
        {
           

            for (int i = 0; i < moverComponentContainer.EntityCount; i++)
            {

                    int entityId = moverComponentContainer.EntityIds[i];
                    var moverComponent = moverComponentContainer.GetComponent(entityId);
                    
                    if (!moverComponent.HasPath) continue;
                    var coordinateComponent = coordinateComponentContainer.GetComponent(entityId);
                    var renderComponent = renderComponetContainer.GetComponent(entityId);

                    if (moverComponent.PathStepNumber != moverComponent.Path.Length)
                    {
                        SetTileOccupant.Invoke(coordinateComponent, -1);
                        coordinateComponent.Coordinate = moverComponent.Path[moverComponent.PathStepNumber];
                        SetTileOccupant.Invoke(coordinateComponent, entityId);

                        renderComponent.TRS = Matrix4x4.TRS(new Vector3(moverComponent.Path[moverComponent.PathStepNumber].x,moverComponent.Path[moverComponent.PathStepNumber].y,0),//* _mapSettings.TileEdgeSize
                                                            Quaternion.identity,
                                                            Vector3.one);
                        
                        
                       // UpdateRenderMatrix.Invoke(ComponentMask.DynamicRenderComponent, i, moverComponent.Path[moverComponent.PathStepNumber]);

                        moverComponent.PathStepNumber++;

                    }
                    else
                    {
                    moverComponent.HasPath = false;
                    moverComponent.Path.Dispose();
                    moverComponent.PathStepNumber = 0;
                       // SetTileOccupant.Invoke(coordinateComponent, entityId);

                    }

                    moverComponentContainer.UpdateComponent(entityId, moverComponent);
                    coordinateComponentContainer.UpdateComponent(entityId, coordinateComponent);
                    renderComponetContainer.UpdateComponent(entityId, renderComponent);
                }
            
        }
    }
}
