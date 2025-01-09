using Game.ECS.Base.Components;
using Game.Factory.Base;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MoverComponentFactory : BaseFactory<MoverComponent>
{
    protected override MoverComponent Create(params object[] arguments)
    {
        return new MoverComponent() { Path = (NativeArray<int2>)arguments[0] };
    }

    protected override MoverComponent Update(ref MoverComponent component, params object[] arguments)
    {
        component.Path = (NativeArray<int2>)arguments[0];
        return component;
    }
}
