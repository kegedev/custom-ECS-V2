using System;
using UnityEngine;

public class SoldierButton : BaseButton
{

    public SoldierType SoldierType;
    public Action<SoldierType> SpawnSoldierAction;
    protected override void InvokeAction()
    {
        SpawnSoldierAction.Invoke(SoldierType);
    }
}
