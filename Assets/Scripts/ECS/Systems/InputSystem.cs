using Game.ECS.Base.Systems;
using Game.ECS.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Game.ECS.Base.Components;
using System.Drawing;
using UnityEngine.EventSystems;

namespace Game.ECS.Systems
{
    public class InputSystem : IInitSystem, IUpdateSystem
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraController _cameraController;
        private ECSWorld _world;


        public Action<int,GameState> ProcessSelection;

        public ushort ActiveStateMask => (ushort)(GameState.Construction | GameState.MainState);

        public InputSystem(Camera camera)
        {
            _camera = camera;
            _cameraController = camera.GetComponent<CameraController>();
        }
        public void Init(SystemManager systemManager)
        {
            _world = systemManager.GetWorld();
        }
        public void Update(SystemManager systemManager)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            _cameraController.MoveCamera(new Vector2(horizontal, vertical));

            if (Input.mouseScrollDelta.y != 0 && !EventSystem.current.IsPointerOverGameObject())
            {
                _cameraController.Zoom(Input.mouseScrollDelta.y);
            }
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {

                GetClickPositionOnXYPlane(Input.mousePosition,systemManager.GetState());
            }
        }


        public void GetClickPositionOnXYPlane(Vector3 screenPosition, GameState gameState)
        {

            Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));


            Vector2 intersection = new Vector2(worldPosition.x , worldPosition.y );
      

            int selectedTileId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)_world.ComponentContainers[typeof(QuadTreeLeafComponent)],
                                       _world.quadTreeNodeDatas,
                                       _world.QuadtreeNodeIndexes,
                                       _world.QuadtreeLeafIndexes,
                                       _world.TileQuadtreeRoot,
                                       intersection);
            ProcessSelection.Invoke(selectedTileId, gameState);
        }

        public Vector2 GetInputPosition()
        {
            Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));


            Vector2 intersection = new Vector2(worldPosition.x + 0.5f, worldPosition.y + 0.5f);

            return intersection;    
        }


    }
}
