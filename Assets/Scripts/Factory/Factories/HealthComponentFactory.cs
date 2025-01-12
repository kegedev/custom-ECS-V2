using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class HealthComponentFactory : BaseFactory<HealthComponent>
{
    protected override HealthComponent Create(params object[] arguments)
    {
        Debug.Log("CREATE HEALTH COMPONENT");
        return new HealthComponent()
        {
            Health = (int)arguments[0],
        };
    }

    protected override HealthComponent Update(ref HealthComponent component, params object[] arguments)
    {
        Debug.Log("UPDATE HEALTH COMPONENT");
        component.Health = (int)arguments[0];
        return component;
    }
}
