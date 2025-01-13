using UnityEngine;
using Game.ECS.Base;
using Game.ECS.Base.Systems;
using Game.Factory;
using Game.Pool;
using Game.ECS.Systems;
using Game.ECS.Base.Components;
using Unity.Mathematics;

public class GameController : MonoBehaviour
{
    private ECSWorld _gameWorld;
    private ECSWorld _uiWorld;
    private SystemManager _systemManager;
    [SerializeField] private UIController _uiController;
    [SerializeField] private Camera _camera;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    private FactoryManager _factoryManager;
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
        MapSettings.Initialize(256, 256, 1,8);
        var poolManager = new PoolManager();
        _factoryManager = new FactoryManager(poolManager);
        _gameWorld = new ECSWorld(256,_factoryManager);
        _systemManager = new SystemManager(_gameWorld);
    }

    private void RegisterSharedData()
    {
        _systemManager.AddSharedData(new MeshContainer { Meshes = new Mesh[] { _mesh } });
        _systemManager.AddSharedData(new MaterialContainer { Materials = new Material[] { _material } });
    }

    private void SetupSystems()
    {
  


        var occupancySystem = new OccupancySystem(_gameWorld);
        var moverCreationSystem = new MoverCreationSystem(_gameWorld, _factoryManager);
        var buildingCreationSystem = new BuildingCreationSystem(_gameWorld,_factoryManager);
        var inputSystem = new InputSystem(_gameWorld,_camera);
        var selectionSystem = new SelectionSystem(_gameWorld);
        var aStarSystem = new AStarSystem(_gameWorld);
        var movementSystem = new MovementSystem(_gameWorld);
        var constructSystem = new ConstructSystem(_gameWorld);
        var areaSystem=new AreaSystem(_gameWorld);
        //var uIController = new UIController(_uiManager);

        ConfigureSystemEventHandlers(occupancySystem, 
                                     moverCreationSystem, 
                                     buildingCreationSystem, 
                                     inputSystem, 
                                     selectionSystem, 
                                     aStarSystem, 
                                     movementSystem, 
                                     constructSystem,
                                     areaSystem
                                     );//uIController

        _systemManager.AddSystem(new TileCreationSystem(_factoryManager));
        _systemManager.AddSystem(new RenderSystem());
        _systemManager.AddSystem(inputSystem);
        _systemManager.AddSystem(new QuadtreeCreationSystem(_factoryManager));
        _systemManager.AddSystem(movementSystem);
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
        ConstructSystem constructSystem,
        AreaSystem areaSystem)
    {
        // MoverCreationSystem
        moverCreationSystem.SetOccupant += occupancySystem.SetTileOccupant;
        moverCreationSystem.GetOccupant += occupancySystem.GetTileOccupant;
        moverCreationSystem.GetSelectedBuildingId += selectionSystem.GetSelectedBuildingId;

        // BuildingCreationSystem
        buildingCreationSystem.CreateArea += areaSystem.CreateArea;
        buildingCreationSystem.DisposeArea += areaSystem.DisposeArea;

        // InputSystem
        inputSystem.ProcessSelection += selectionSystem.ProcessSelection;

        // SelectionSystem
        selectionSystem.SetMoverPath += movementSystem.SetMoverPath;
        selectionSystem.TryToConstruct += constructSystem.TryToConstruct;
        selectionSystem.BuildingSelected += _uiController.OnBuildingSelected;
        selectionSystem.SoldierSelected += _uiController.OnSoldierSelected;

        // MovementSystem
        movementSystem.GetMoverPath += aStarSystem.GetMoverPath;
        movementSystem.SetTileOccupant += occupancySystem.SetTileOccupant;

        // ConstructSystem
        constructSystem.GetInputPos += inputSystem.GetInputPosition;
        constructSystem.SetGameState += _systemManager.UpdateGameState;
        constructSystem.SetGameState += movementSystem.UpdateGameState;
        constructSystem.ConstructBuilding += buildingCreationSystem.CreateBuilding;

        //AreaSystem
        areaSystem.SetOccupant += occupancySystem.SetTileOccupant;
        areaSystem.GetOccupant += occupancySystem.GetTileOccupant;

        // UIManager
        _uiController.SpawnSoldier += moverCreationSystem.CreateMover;
        _uiController.ConstructBuilding += constructSystem.BuildingSelectedToConstruct;

        //world
        _gameWorld.DisposeArea += buildingCreationSystem.DisposeArea;
        _gameWorld.SetOccupant += occupancySystem.SetTileOccupant;


    }
}
