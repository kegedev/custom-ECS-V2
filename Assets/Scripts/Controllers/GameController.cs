using UnityEngine;
using Game.ECS.Base;
using Game.ECS.Base.Systems;
using Game.Factory;
using Game.Pool;
using Game.ECS.Systems;
using Game.ECS.Base.Components;

public class GameController: MonoBehaviour
{
    private ECSWorld _gameWorld;
    private ECSWorld _UIWorld;
    private SystemManager _systemManager;
    [SerializeField] UIManager _uiManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    void Start()
    {

        MapSettings.Initialize(128,128,1,8);

        _gameWorld = new ECSWorld(256);
        _systemManager = new SystemManager(_gameWorld);

        PoolManager poolManager = new PoolManager();
        FactoryManager factoryManager = new FactoryManager(poolManager);


        // _systemManager.AddSystem(new AssetLoadingSystem());
        _systemManager.AddSharedData(new MeshContainer()
        {
            Meshes = new Mesh[] { _mesh }
        });
        _systemManager.AddSharedData(new MaterialContainer()
        {
            Materials = new Material[] { _material }
        });

        OccupancySystem occupancySystem = new OccupancySystem();
        MoverCreationSystem moverCreationSystem = new MoverCreationSystem(factoryManager);
        moverCreationSystem.SetOccupant += occupancySystem.SetTileOccupant;
        moverCreationSystem.GetOccupant += occupancySystem.GetTileOccupant;

        BuildingCreationSystem buildingCreationSystem = new BuildingCreationSystem(factoryManager);
        buildingCreationSystem.SetOccupant += occupancySystem.SetTileOccupant;
        buildingCreationSystem.GetOccupant += occupancySystem.GetTileOccupant;

        InputSystem inputSystem = new InputSystem(_camera);
        SelectionSystem selectionSystem = new SelectionSystem();
        inputSystem.ProcessSelection += selectionSystem.ProcessSelection;

        AStarSystem aStarSystem= new AStarSystem();
        selectionSystem.GetMoverPath += aStarSystem.GetMoverPath;

        MovementSystem movementSystem = new MovementSystem();
        movementSystem.SetTileOccupant += occupancySystem.SetTileOccupant;

        ConstructSystem constructSystem = new ConstructSystem();
        constructSystem.GetInputPos += inputSystem.GetInputPosition;

        _systemManager.AddSystem(occupancySystem);
        _systemManager.AddSystem(new TileCreationSystem(factoryManager));
        _systemManager.AddSystem(new RenderSystem());
        _systemManager.AddSystem(inputSystem);
        _systemManager.AddSystem(new QuadtreeCreationSystem(factoryManager));
        _systemManager.AddSystem(selectionSystem);
        _systemManager.AddSystem(moverCreationSystem);
        _systemManager.AddSystem(aStarSystem);
        _systemManager.AddSystem(movementSystem);
        _systemManager.AddSystem(buildingCreationSystem);
        _systemManager.AddSystem(constructSystem);
        _systemManager.InitSystems();

        Debug.Log(_gameWorld.ComponentContainers.Count);

        Debug.Log(_gameWorld.QuadtreeNodeIndexes.Length);
        Debug.Log(_gameWorld.QuadtreeLeafIndexes.Length);

  
    }

    private void Update()
    {
        _systemManager.UpdateSystems();
    }


}
