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
    public Action<SoldierType> SpawnSoldier;


    public void SelectConstructBarrack()
    {
        ConstructBuilding.Invoke(BuildingType.Barrack);
    }

    public void SelectConstructPowerPlant()
    {
        ConstructBuilding.Invoke(BuildingType.PowerPlant);
    }

    public void SpawnSoldier1()
    {
        SpawnSoldier.Invoke(SoldierType.Soldier1);
    }
    public void SpawnSoldier2()
    {
        SpawnSoldier.Invoke(SoldierType.Soldier2);
    }
    public void SpawnSoldier3()
    {
        SpawnSoldier.Invoke(SoldierType.Soldier3);
    }

    public void ShowSelectedBuilding(BuildingType buildingType,int health)
    {
        if(buildingType==BuildingType.Barrack)
        {
            _selectedUnitName.text = "BARRACK";
            _selectedUnitProduction.SetActive(true);
        }
        else
        {
            _selectedUnitName.text = "POWER PLANT";
            _selectedUnitProduction.SetActive(false);
        }
        _selectedUnitDamage.gameObject.SetActive(false);
        _selectedUnitHealth.text=health.ToString();
        _selectedUnitImage.material.SetTextureOffset("_MainTex", MapConstants.BuildingOffsets[buildingType]);
    }

    public void ShowSelectedSoldier(SoldierType soldierType,int health, int damage)
    {
        _selectedUnitProduction.SetActive(false);

        switch (soldierType)
        {
            case SoldierType.None:
                break;
            case SoldierType.Soldier1:
                _selectedUnitName.text = "Soldier1";
                break;
            case SoldierType.Soldier2:
                _selectedUnitName.text = "Soldier2";
                break;
            case SoldierType.Soldier3:
                _selectedUnitName.text = "Soldier3";
                break;
            default:
                break;
        }
        _selectedUnitDamage.gameObject.SetActive(false);
        _selectedUnitDamage.text= damage.ToString();
        _selectedUnitHealth.text = health.ToString();
        _selectedUnitImage.material.SetTextureOffset("_MainTex", MapConstants.SoldierOffsets[soldierType]);
    }
}
