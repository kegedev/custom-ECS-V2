using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class AttackComponentFactory : BaseFactory<AttackComponent>
{
    protected override AttackComponent Create(params object[] arguments)
    {
        return new AttackComponent()
        {
            Damage = (int)arguments[0],
            TargetId = (int)arguments[1],
        };
    }

    protected override AttackComponent Update(ref AttackComponent component, params object[] arguments)
    {
        component.Damage = (int)arguments[0];
        component.TargetId = (int)arguments[1];
        return component;
    }
}
