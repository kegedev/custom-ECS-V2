using UnityEngine;

public class PowerPlantButtonFactory : ObjectFactory<BuildingButton>
{
    public PowerPlantButtonFactory(GameObject prefab) : base(prefab)
    {
    }

    public BuildingButton GetPowerPlantButton()
    {
        BuildingButton buildingButton = Create();
        buildingButton.BuildingType = BuildingType.PowerPlant;
        return buildingButton;
    }
}
