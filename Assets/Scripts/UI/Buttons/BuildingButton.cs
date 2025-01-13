using System;
using UnityEngine;

public class BuildingButton : BaseButton
{
    public BuildingType BuildingType;
    public Action<BuildingType> ConstructBuildingAction;
    protected override void InvokeAction()
    {
        ConstructBuildingAction.Invoke(BuildingType);
    }
}
