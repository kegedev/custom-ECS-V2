using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class TileComponentFactory : BaseFactory<TileComponent>
{
    protected override TileComponent Create(params object[] arguments)
    {
        return new TileComponent()
        {
            MoverIndex = (int)arguments[0]
        };
    }

    protected override TileComponent Update(ref TileComponent component, params object[] arguments)
    {
        component.MoverIndex = (int)arguments[0];
        return component;
    }
}

