using Game.ECS.Base.Components;
using Game.Factory.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeLeafComponentFactory : BaseFactory<QuadTreeLeafComponent>
{
    protected override QuadTreeLeafComponent Create(params object[] arguments)
    {
        return new QuadTreeLeafComponent()
        {
            LeafID = (int)arguments[0],
            Rect = (Rect)arguments[1]
        };
    }

    protected override QuadTreeLeafComponent Update(ref QuadTreeLeafComponent quadtreeLeafComponent, params object[] arguments)
    {
        quadtreeLeafComponent.LeafID = (int)arguments[0];
        quadtreeLeafComponent.Rect = (Rect)arguments[1];

        return quadtreeLeafComponent;
    }
}
