using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using Game.Factory;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.Systems
{
    public class MoverCreationSystem : IInitSystem
    {

        private FactoryManager _factoryManager;
        public Action<CoordinateComponent, int> SetOccupant;
        public Func<CoordinateComponent, int> GetOccupant;

        public MoverCreationSystem(FactoryManager factoryManager)
        {
            _factoryManager = factoryManager;
        }
        public void Init(SystemManager systemManager)
        {
            ECSWorld eCSWorld = systemManager.GetWorld();

            CreateMovers(eCSWorld);
        }

        public void CreateMovers(ECSWorld eCSWorld)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int absoluteX = UnityEngine.Random.Range(0,120);
                    int absoluteY = UnityEngine.Random.Range(0, 120);
                    int2 coordinate = new int2(absoluteX, absoluteY);
                    Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(absoluteX, absoluteY, 0), Quaternion.identity, Vector3.one * 0.95f);

                    if (absoluteX >= MapSettings.MapWidth || absoluteY >= MapSettings.MapHeight)
                        continue;

                    int newEntityID = eCSWorld.CreateNewEntity();
                   
                    CoordinateComponent coordinateComponent = _factoryManager.GetInstance<CoordinateComponent>(coordinate);

                    eCSWorld.AddComponentToEntity<CoordinateComponent>(newEntityID,
                                                                       ComponentMask.CoordinateComponent,
                                                                       coordinateComponent);

                    eCSWorld.AddComponentToEntity<MoverComponent>(newEntityID,
                                                                  ComponentMask.MoverComponent,
                                                                  _factoryManager.GetInstance<MoverComponent>(new object[] { false, 0, new NativeArray<int2>() }));

                    eCSWorld.AddComponentToEntity<RenderComponent>(newEntityID,
                                                                 ComponentMask.RenderComponent,
                                                                  _factoryManager.GetInstance<RenderComponent>(new object[2] { matrix, ((absoluteX + absoluteY) % 2 == 0) ? MapConstants.SoldierOffsets[SoldierType.Soldier1] : MapConstants.SoldierOffsets[SoldierType.Soldier2] }));
                    SetOccupant.Invoke(coordinateComponent, newEntityID);
                }
            }
           
        }
    }

}
