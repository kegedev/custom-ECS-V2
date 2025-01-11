using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class BuildingComponentFactory : BaseFactory<BuildingComponent>
{
    protected override BuildingComponent Create(params object[] arguments)
    {
        return new BuildingComponent()
        {
            BuildingType = (ushort)arguments[0],
        };
    }

    protected override BuildingComponent Update(ref BuildingComponent component, params object[] arguments)
    {
        component.BuildingType = (ushort)arguments[0];
        return component;
    }
}
