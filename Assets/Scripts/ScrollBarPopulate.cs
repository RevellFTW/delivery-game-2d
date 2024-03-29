using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarPopulate : MonoBehaviour
{
    //todo: populate the upgrades and the delivery rounds.
    public List<string> firstData;
    public List<string> secondData;

    public static GameObject prefab; // reference to the prefab that will be instantiated
    public static GameObject upgradePrefab; // reference to the prefab that will be instantiated
    public static GameObject BottomButtons; // reference to the prefab that will be instantiated
    public static GameObject listOfUpgrades;
    public static GameObject listOfPackages;
    public static RectTransform content; // reference to the content transform of the scroll view
    public static RectTransform upgradeContent; // reference to the content transform of the scroll view
    public static GameObject scroll;
    // public int numberOfItems = 20; // number of items to display in the scroll view
    // Start is called before the first frame update

    public static GameObject GUI;
    public static GameObject UpgradeGUI;

    void Start()
    {
        GUI = GameObject.Find("DepoGUI");
        scroll = GameObject.Find("Scroll");
        prefab = GameObject.Find("Button (1)");
        upgradePrefab = GameObject.Find("UpgradeButton");
        content = GameObject.Find("ListOfPackages").GetComponent<RectTransform>();
        upgradeContent = GameObject.Find("ListOfUpgrades").GetComponent<RectTransform>();
        listOfUpgrades = GameObject.Find("ListOfUpgrades");
        listOfPackages = GameObject.Find("ListOfPackages");
        BottomButtons = GameObject.Find("BottomButtons");

        //foreach (PackageRound packageRound in Delivery.packageRounds)
        //{
        //    GameObject listItem = Instantiate(prefab, content.transform);
        //    TMP_Text itemText = listItem.GetComponentInChildren<TMP_Text>();
        //    itemText.text = packageRound.Packages.Count + " packages, complexity: " + packageRound.complexity;
        //}
        //  UpgradeGUI.SetActive(false);

        //Instantiate(upgradePrefab, upgradeContent);
    }

  
    
    public static void AddToList(PackageRound packageRound)
    {
        GameObject listItem = Instantiate(prefab, content.transform);
        listItem.GetComponent<PackageRound>().Packages = packageRound.Packages;
        listItem.GetComponent<PackageRound>().complexity = packageRound.complexity;
        TMP_Text[] itemTexts = listItem.GetComponentsInChildren<TMP_Text>();
        itemTexts[0].text = packageRound.Packages.Count + " packages";
        itemTexts[1].text = packageRound.GetComplexity();
    }
}
