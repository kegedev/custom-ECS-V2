using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController:MonoBehaviour
{
    [SerializeField] private GameUI _view;
    [SerializeField] private GameObject _barrackButton;
    [SerializeField] private GameObject _powerplantButton;
    [SerializeField] private GameObject _soldierButton;
    [SerializeField] private GameObject _buildingButtonParent;
    [SerializeField] private GameObject _soldierButtonParent;
    [SerializeField] private Image _selectedUnitImage;
    [SerializeField] private InfiniteScrollView _infiniteScrollView;
    public Action<BuildingType> ConstructBuilding;
    public Action<SoldierType> SpawnSoldier;
    private BarrackButtonFactory _barrackButtonFactory;
    private PowerPlantButtonFactory _powerplantButtonFactory;
    private SoldierButtonFactory _soldierButtonFactory;
    

    private void Awake()
    {
        _barrackButtonFactory = new BarrackButtonFactory(_barrackButton);
        _powerplantButtonFactory = new PowerPlantButtonFactory(_powerplantButton);
        _soldierButtonFactory = new SoldierButtonFactory(_soldierButton);
        _infiniteScrollView.RecycleButton += RecycleButton;
        _infiniteScrollView.GetScrollElement += OnGetScrollElement;
        _infiniteScrollView.GetBuildingButton += OnGetBuildingButton;
        CreateButtons();
        _view.ActivationSoldierButtons(false);
    }

    private void RecycleButton(GameObject button)
    {
        switch (button.GetComponent<BuildingButton>().BuildingType)
        {
            case BuildingType.Barrack:
                _barrackButtonFactory.Recycle(button);
                break;
            case BuildingType.PowerPlant:
                _powerplantButtonFactory.Recycle(button);
                break;
            default:
                break;
        }

    }

    private GameObject OnGetBuildingButton(BuildingType buildingType)
    {
        switch (buildingType)
        {
            case BuildingType.Barrack:
                return CreateBarrackButton();
            case BuildingType.PowerPlant:
                return CreatePowerplantButton();
            default:
                return CreateBarrackButton();
         
        }
    }

    private GameObject OnGetScrollElement(GameObject element = null)
    {
        switch (element.GetComponent<BuildingButton>().BuildingType)
        {
            case BuildingType.Barrack:
                _barrackButtonFactory.Recycle(element);
                return CreatePowerplantButton();
            case BuildingType.PowerPlant:
                _powerplantButtonFactory.Recycle(element);
                return CreateBarrackButton();
            default:
                return null;
        }



    }


    public void CreateButtons()
    {
        CreateSoldierButtons();
    }

    private GameObject CreateBarrackButton()
    {
        BuildingButton buildingButton = _barrackButtonFactory.GetBarrackButton();
       // buildingButton.transform.SetParent(_buildingButtonParent.transform);
        buildingButton.ConstructBuildingAction += OnBuildingButtonClicked;
        return buildingButton.gameObject;
    }
    private GameObject CreatePowerplantButton()
    {
        BuildingButton buildingButton = _powerplantButtonFactory.GetPowerPlantButton();
       // buildingButton.transform.SetParent( _buildingButtonParent.transform);
        buildingButton.ConstructBuildingAction += OnBuildingButtonClicked;
        return buildingButton.gameObject;
    }

    private void CreateSoldierButtons()
    {
        foreach (SoldierType soldierType in Enum.GetValues(typeof(SoldierType)))
        {
            if (soldierType == SoldierType.None) continue;
            SoldierButton soldierButton = _soldierButtonFactory.GetSoldierButton(soldierType);
            soldierButton.transform.SetParent(_soldierButtonParent.transform);
            soldierButton.SoldierType = soldierType;
            soldierButton.SpawnSoldierAction += OnSoldierButtonClicked;
        }
    }

    public void OnBuildingButtonClicked(BuildingType buildingType)
    {
        ConstructBuilding.Invoke(buildingType);
    }

    public void OnSoldierButtonClicked(SoldierType soldierType)
    {
        SpawnSoldier.Invoke(soldierType);

    }

    public void OnBuildingSelected(BuildingType buildingType, int health)
    {

        string buildingName = buildingType == BuildingType.Barrack ? "BARRACK" : "POWER PLANT";
        Vector2 textureOffset = MapConstants.BuildingOffsets[buildingType];
        _selectedUnitImage.material.SetTextureOffset("_MainTex", textureOffset);
        _view.ShowSelectedBuilding(buildingName, health, buildingType == BuildingType.Barrack);
    }

    public void OnSoldierSelected(SoldierType soldierType, int health, int damage)
    {
       
        string soldierName = soldierType.ToString();
        Vector2 textureOffset = MapConstants.SoldierOffsets[soldierType];
        _selectedUnitImage.material.SetTextureOffset("_MainTex", textureOffset);
        _view.ShowSelectedSoldier(soldierName, health, damage);
    }
}
