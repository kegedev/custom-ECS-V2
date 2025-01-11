using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _selectedUnitName;
    [SerializeField] TextMeshProUGUI _selectedUnitHealth;
    [SerializeField] TextMeshProUGUI _selectedUnitDamage;

    [SerializeField] Image _selectedUnitImage;

    [SerializeField] GameObject _selectedUnitProduction;

    public Action<BuildingType> ConstructBuilding;


    public void SelectConstructBarrack()
    {
        ConstructBuilding.Invoke(BuildingType.Barrack);
    }

    public void SelectConstructPowerPlant()
    {
        ConstructBuilding.Invoke(BuildingType.PowerPlant);
    }

}
