using UnityEngine;
using Game.ECS;
public class GameController : MonoBehaviour
{
    private ECSWorld _gameWorld;

    void Start()
    {
        _gameWorld = new ECSWorld();

    }


}
