using UnityEngine;
using Game.ECS.Base;
using Game.ECS.Base.Systems;
using Game.Factory;
using Game.Pool;
using Game.ECS.Systems;
using Game.ECS.Base.Components;

public class GameController : MonoBehaviour
{
    private ECSWorld _gameWorld;
    private ECSWorld _uiWorld;
    private SystemManager _systemManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;

    private void Start()
    {
        InitializeGame();
        RegisterSharedData();
        SetupSystems();
    }

    private void Update()
    {
        _systemManager.UpdateSystems();
    }

    private void InitializeGame()
    {
        MapSettings.Initialize(128, 128, 1, 8);
        _gameWorld = new ECSWorld(256);
        _systemManager = new SystemManager(_gameWorld);
    }

    private void RegisterSharedData()
    {
        _systemManager.AddSharedData(new MeshContainer { Meshes = new Mesh[] { _mesh } });
        _systemManager.AddSharedData(new MaterialContainer { Materials = new Material[] { _material } });
    }

    private void SetupSystems()
    {
        var poolManager = new PoolManager();
        var factoryManager = new FactoryManager(poolManager);


        var occupancySystem = new OccupancySystem();
        var moverCreationSystem = new MoverCreationSystem(factoryManager);
        var buildingCreationSystem = new BuildingCreationSystem(factoryManager);
        var inputSystem = new InputSystem(_camera);
        var selectionSystem = new SelectionSystem();
        var aStarSystem = new AStarSystem();
        var movementSystem = new MovementSystem();
        var constructSystem = new ConstructSystem();


        ConfigureSystemEventHandlers(occupancySystem, moverCreationSystem, buildingCreationSystem, inputSystem, selectionSystem, aStarSystem, movementSystem, constructSystem);


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
        _systemManager.AddSystem(new AttackSystem());

        _systemManager.InitSystems();
    }

    private void ConfigureSystemEventHandlers(
        OccupancySystem occupancySystem,
        MoverCreationSystem moverCreationSystem,
        BuildingCreationSystem buildingCreationSystem,
        InputSystem inputSystem,
        SelectionSystem selectionSystem,
        AStarSystem aStarSystem,
        MovementSystem movementSystem,
        ConstructSystem constructSystem)
    {
        // MoverCreationSystem
        moverCreationSystem.SetOccupant += occupancySystem.SetTileOccupant;
        moverCreationSystem.GetOccupant += occupancySystem.GetTileOccupant;
        moverCreationSystem.GetSelectedBuildingId += selectionSystem.GetSelectedBuildingId;

        // BuildingCreationSystem
        buildingCreationSystem.SetOccupant += occupancySystem.SetTileOccupant;
        buildingCreationSystem.GetOccupant += occupancySystem.GetTileOccupant;

        // InputSystem
        inputSystem.ProcessSelection += selectionSystem.ProcessSelection;

        // SelectionSystem
        selectionSystem.GetMoverPath += aStarSystem.GetMoverPath;
        selectionSystem.TryToConstruct += constructSystem.TryToConstruct;
        selectionSystem.BuildingSelected += _uiManager.ShowSelectedBuilding;
        selectionSystem.SoldierSelected += _uiManager.ShowSelectedSoldier;

        // MovementSystem
        movementSystem.SetTileOccupant += occupancySystem.SetTileOccupant;

        // ConstructSystem
        constructSystem.GetInputPos += inputSystem.GetInputPosition;
        constructSystem.SetGameState += _systemManager.UpdateGameState;
        constructSystem.ConstructBuilding += buildingCreationSystem.CreateBuilding;
        _uiManager.ConstructBuilding += constructSystem.BuildingSelectedToConstruct;

        // UIManager
        _uiManager.SpawnSoldier += moverCreationSystem.CreateMover;
    }
}
