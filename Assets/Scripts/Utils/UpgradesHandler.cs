using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public class UpgradesHandler : MonoBehaviour
    {
        #region Upgrade buttons
        public static GameObject upgradeStorage;
        public static GameObject upgradeSpeed;
        public static GameObject upgradeFuelIntensity;
        public static GameObject buyNavigation;
        #endregion


        private void Start()
        {
            upgradeStorage = GameObject.Find("UpgradeButton");
            upgradeSpeed = GameObject.Find("UpgradeSpeed");
            upgradeFuelIntensity = GameObject.Find("UpgradeFuel");
            buyNavigation = GameObject.Find("BuyNavigation");
        }

        void Update()
        {
             
            if (Delivery.money < 100)
            {
                upgradeStorage.GetComponent<Button>().interactable = false;
                upgradeSpeed.GetComponent<Button>().interactable = false;
                upgradeFuelIntensity.GetComponent<Button>().interactable = false;
                buyNavigation.GetComponent<Button>().interactable = false;
            }
            else if (Delivery.money >= Delivery.speedCost)
            {
                upgradeSpeed.GetComponent<Button>().interactable = true;
            }
            else if (Delivery.money >= Delivery.fuelCost)
            {
                upgradeFuelIntensity.GetComponent<Button>().interactable = true;

            }
            else if (Delivery.money >= Delivery.storageCost)
            {
                upgradeStorage.GetComponent<Button>().interactable = true;

            }
            else if (Delivery.money >= 5000)
            {
                buyNavigation.GetComponent<Button>().interactable = true;
            }
            upgradeStorage.GetComponentInChildren<TMP_Text>().text = "UPGRADE storage: " + Delivery.storageCost.ToString() + "$";
            upgradeSpeed.GetComponentInChildren<TMP_Text>().text = "UPGRADE speed: " + Delivery.speedCost.ToString() + "$"; ;
            upgradeFuelIntensity.GetComponentInChildren<TMP_Text>().text = "UPGRADE Fuel Intensity: " + Delivery.fuelCost.ToString() + "$";
        }
    }
}
