using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class BuildingComponentFactory : BaseFactory<BuildingComponent>
{
    protected override BuildingComponent Create(params object[] arguments)
    {
        return new BuildingComponent()
        {
            
        };
    }

    protected override BuildingComponent Update(ref BuildingComponent component, params object[] arguments)
    {
      
        return component;
    }
}
