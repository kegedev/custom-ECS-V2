using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class SoldierComponentFactory : BaseFactory<SoldierComponent>
{
    protected override SoldierComponent Create(params object[] arguments)
    {
        return new SoldierComponent()
        {
            SoldierType = (ushort)arguments[0],
        };
    }

    protected override SoldierComponent Update(ref SoldierComponent component, params object[] arguments)
    {
        component.SoldierType = (ushort)arguments[0];
        return component;
    }
}
