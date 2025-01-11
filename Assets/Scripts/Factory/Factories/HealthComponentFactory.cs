using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class HealthComponentFactory : BaseFactory<HealthComponent>
{
    protected override HealthComponent Create(params object[] arguments)
    {
        return new HealthComponent()
        {
            Health = (int)arguments[0],
        };
    }

    protected override HealthComponent Update(ref HealthComponent component, params object[] arguments)
    {
        component.Health = (int)arguments[0];
        return component;
    }
}
