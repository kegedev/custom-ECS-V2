using Game.ECS.Base.Components;
using Game.Factory.Base;
using Unity.Mathematics;
using UnityEngine;

public class RenderComponentFactory : BaseFactory<RenderComponent>
{
    protected override RenderComponent Create(params object[] arguments)
    {
        return new RenderComponent()
        {
            TRS = (Matrix4x4)arguments[0],
            TextureOffset=(float2) arguments[1]
        };
    }

    protected override RenderComponent Update(ref RenderComponent component, params object[] arguments)
    {
        component.TRS = (Matrix4x4)arguments[0];
        component.TextureOffset = (float2)arguments[1];
        return component;
    }
}
