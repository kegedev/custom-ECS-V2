using UnityEngine;

public class SoldierButtonFactory : ObjectFactory<SoldierButton>
{
    public SoldierButtonFactory(GameObject prefab) : base(prefab)
    {
    }

    public SoldierButton GetSoldierButton(SoldierType soldierType)
    {
        SoldierButton SoldierButton = Create();
        SoldierButton.SoldierType = soldierType;
        return SoldierButton;
    }
}
