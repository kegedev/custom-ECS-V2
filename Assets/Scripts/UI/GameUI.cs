using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

        [SerializeField] private TextMeshProUGUI _selectedUnitName;
        [SerializeField] private TextMeshProUGUI _selectedUnitHealth;
        [SerializeField] private TextMeshProUGUI _selectedUnitDamage;
        [SerializeField] private GameObject _soldierButtonParent;
        

        public void ShowSelectedBuilding(string buildingName, int health, bool showProductionMenu)
        {
            ActivationSoldierButtons(showProductionMenu);
            _selectedUnitDamage.text = "";
            _selectedUnitName.text = buildingName;
            _selectedUnitHealth.text = "HP " + health.ToString();
        }

        public void ShowSelectedSoldier(string soldierName, int health, int damage)
        {
            ActivationSoldierButtons(false);
            _selectedUnitName.text = soldierName;
            _selectedUnitHealth.text = "HP " + health.ToString();
            _selectedUnitDamage.text = "DMG " + damage.ToString();
        }
    
        public void ActivationSoldierButtons(bool isActive)
        {
            _soldierButtonParent.SetActive(isActive);
        }

   
}

