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
    private SystemManager _systemManager;
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
        _systemManager.AddSystem(new TileCreationSystem(factoryManager));
        _systemManager.AddSystem(new RenderSystem());

        _systemManager.InitSystems();

        Debug.Log(_gameWorld.ComponentContainers.Count);
      
            ComponentContainer<RenderComponent> container = (ComponentContainer<RenderComponent>)_gameWorld.ComponentContainers[ComponentMask.RenderComponent];

            
            Debug.Log(container.EntityCount);

            foreach (var item1 in container.Components)
            {
                Debug.Log(item1.TRS); 
                
            }

        //ComponentContainer<TileComponent> containert = (ComponentContainer<TileComponent>)_gameWorld.ComponentContainers[ComponentMask.TileComponent];


        //Debug.Log(containert.EntityCount);

        //foreach (var item1 in containert.Components)
        //{
        //    Debug.Log(item1.MoverIndex);

        //}
    }

    private void Update()
    {
        _systemManager.UpdateSystems();
    }
}
