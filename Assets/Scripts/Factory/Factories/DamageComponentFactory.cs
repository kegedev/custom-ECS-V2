using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class DamageComponentFactory : BaseFactory<DamageComponent>
{
    protected override DamageComponent Create(params object[] arguments)
    {
        return new DamageComponent()
        {
            Damage = (int)arguments[0],
        };
    }

    protected override DamageComponent Update(ref DamageComponent component, params object[] arguments)
    {
        component.Damage = (int)arguments[0];
        return component;
    }
}
