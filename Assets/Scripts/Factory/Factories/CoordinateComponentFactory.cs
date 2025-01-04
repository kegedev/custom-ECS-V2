using Game.ECS.Base.Components;
using Game.Factory.Base;
using Unity.Mathematics;
using UnityEngine;

public class CoordinateComponentFactory : BaseFactory<CoordinateComponent>
{
    protected override CoordinateComponent Create(params object[] arguments)
    {
        return new CoordinateComponent() { Coordinate = (int2)arguments[0] };
    }

    protected override CoordinateComponent Update(ref CoordinateComponent component, params object[] arguments)
    {
        component.Coordinate = (int2)arguments[0];
        return component;
    }
}
