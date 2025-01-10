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

namespace Game.ECS.Systems
{
    public class InputSystem : IInitSystem, IUpdateSystem
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraController _cameraController;
        private ECSWorld _world;


        public Action<int> ProcessSelection;

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

            if (Input.mouseScrollDelta.y != 0)
            {
                _cameraController.Zoom(Input.mouseScrollDelta.y);
            }
            if (Input.GetMouseButtonUp(0))
            {

                GetClickPositionOnXYPlane(Input.mousePosition);
            }
        }


        public void GetClickPositionOnXYPlane(Vector3 screenPosition)
        {

            Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));


            Vector2 intersection = new Vector2(worldPosition.x + 0.5f, worldPosition.y + 0.5f);
            Debug.Log("Input point: " + intersection);

            int selectedTileId = QuerySystem.GetEntityId((ComponentContainer<QuadTreeLeafComponent>)_world.ComponentContainers[ComponentMask.QuadTreeLeafComponent],
                                       _world.quadTreeNodeDatas,
                                       _world.QuadtreeNodeIndexes,
                                       _world.QuadtreeLeafIndexes,
                                       _world.TileQuadtreeRoot,
                                       intersection);
            ProcessSelection.Invoke(selectedTileId);
        }


    }
}
