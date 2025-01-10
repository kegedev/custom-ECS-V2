using Game.ECS.Base.Components;
using Game.Factory.Base;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class AreaComponentFactory : BaseFactory<AreaComponent>
{
    protected override AreaComponent Create(params object[] arguments)
    {
        return new AreaComponent()
        {
            Width = (int)arguments[0],
            Height = (int)arguments[1],

        };
    }

    protected override AreaComponent Update(ref AreaComponent component, params object[] arguments)
    {
        component.Width = (int)arguments[0];
        component.Height = (int)arguments[1];

        return component;
    }
}
