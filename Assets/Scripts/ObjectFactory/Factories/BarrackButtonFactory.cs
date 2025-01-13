using UnityEngine;

public class BarrackButtonFactory : ObjectFactory<BuildingButton>
{
    public BarrackButtonFactory(GameObject prefab) : base(prefab)
    {
    }

    public BuildingButton GetBarrackButton()
    {
        BuildingButton buildingButton= Create();
        buildingButton.BuildingType = BuildingType.Barrack;
        return buildingButton;
    }
}
