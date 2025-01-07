using Game.ECS.Base.Components;
using Game.Factory.Base;
using UnityEngine;

public class RenderComponentFactory : BaseFactory<RenderComponent>
{
    protected override RenderComponent Create(params object[] arguments)
    {
        return new RenderComponent()
        {
            TRS = (Matrix4x4)arguments[0],
            TextureOffset=(float) arguments[1]
        };
    }

    protected override RenderComponent Update(ref RenderComponent component, params object[] arguments)
    {
        component.TRS = (Matrix4x4)arguments[0];
        component.TextureOffset = (float)arguments[1];
        return component;
    }
}
