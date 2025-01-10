using Game.ECS.Base.Components;
using Game.Factory.Base;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MoverComponentFactory : BaseFactory<MoverComponent>
{
    protected override MoverComponent Create(params object[] arguments)
    {
        return new MoverComponent()
        {
            HasPath = (bool)arguments[0],
            PathStepNumber = (int)arguments[1],
            Path = (NativeArray<int2>)arguments[2],

        };
    }

    protected override MoverComponent Update(ref MoverComponent component, params object[] arguments)
    {
        component.HasPath = (bool)arguments[0];
        component.PathStepNumber = (int)arguments[1];
        component.Path = (NativeArray<int2>)arguments[2];

        return component;
    }
}
