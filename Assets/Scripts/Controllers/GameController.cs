using UnityEngine;
using Game.ECS.Base;
using Game.ECS.Base.System;

public class GameController : MonoBehaviour
{
    private ECSWorld _gameWorld;
    private SystemManager _systemManager;
    void Start()
    {
        _gameWorld = new ECSWorld();
        _systemManager = new SystemManager(_gameWorld);
    }


}
