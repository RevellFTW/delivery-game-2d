using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Delivery : MonoBehaviour
{

    public float money = 0;
    public static float fuel;
    private static float fuelIntensity;


    [SerializeField] Color32 hasPackageColor = new Color32(1, 1, 1, 1);
    [SerializeField] Color32 noPackageColor = new Color32(0, 1, 0, 1);


    public int maxPackages = 5; // Maximum number of packages that the player can carry at a time
    public bool hasPackage = false; // Whether the player is currently carrying a package

    public List<Package> depoPackages = new List<Package>();
    private SpriteRenderer spriteRenderer; // Reference to the player's sprite renderer component

    #region prefabs & packages

    public GameObject packagePrefab;
    public GameObject customerPrefab;
    public GameObject moneyPrefab;
    public static GameObject fuelPrefab;
    public static GameObject storagePrefab;
    public static GameObject fuelEfficiencyPrefab;
    public static GameObject speedometerPrefab;
    public GameObject fuelRefillPrefab;
    [SerializeField]
    public static List<PackageRound> packageRounds;
    private int minPickups = 5;
    private int maxPickups = 10;
    #endregion

    #region Colors

    public static Color fullFuel = new Color(0, 255, 0);
    public static Color mediumFuel = new Color(255, 215, 0);
    public static Color lowFuel = Color.red;

    #endregion
    #region Pickup And Delivery Locations
    public List<Vector2> pickupLocations;
    public List<Vector2> deliveryLocations;
    #endregion

    #region game state
    [SerializeField]
    public bool DeliveryState = false;
    //rename this, its false if we picked up a package, and after we can enter the depo.
    public bool TouchingDepo = false;
    #endregion
    [SerializeField]
    private static PackageRound currentRound;
    [SerializeField]
    private static float currentRoundMultiplier = 1;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fuelPrefab = GameObject.Find("Fuel");
        storagePrefab = GameObject.Find("Storage");
        fuelEfficiencyPrefab = GameObject.Find("Mileage");
        speedometerPrefab = GameObject.Find("Speedometer");
        fuel = 100;
        fuelIntensity = 0.003f;
        currentRound = new PackageRound();

        packageRounds = new List<PackageRound>(){
            new PackageRound() };

        //TODO: REFACTOR IF EVERYTHING ELSE WORKS
        pickupLocations = new List<Vector2>()
        {
            GameObject.Find("Package (1)").transform.position,
            GameObject.Find("Package (2)").transform.position,
            GameObject.Find("Package (3)").transform.position,
            GameObject.Find("Package (4)").transform.position,
            GameObject.Find("Package (5)").transform.position,
            GameObject.Find("Package (6)").transform.position,
            GameObject.Find("Package (7)").transform.position,
            GameObject.Find("Package (8)").transform.position,
            GameObject.Find("Package (9)").transform.position,
            GameObject.Find("Package (10)").transform.position,
            GameObject.Find("Package (11)").transform.position,
            GameObject.Find("Package (12)").transform.position,
            GameObject.Find("Package (13)").transform.position,
            GameObject.Find("Package (14)").transform.position,
            GameObject.Find("Package (15)").transform.position,
            GameObject.Find("Package (16)").transform.position,
            GameObject.Find("Package (17)").transform.position,
            GameObject.Find("Package (18)").transform.position,
            GameObject.Find("Package (19)").transform.position,
            GameObject.Find("Package (20)").transform.position,
            GameObject.Find("Package (21)").transform.position,
            GameObject.Find("Package (22)").transform.position,
            GameObject.Find("Package (23)").transform.position,
            GameObject.Find("Package (24)").transform.position
            };


        deliveryLocations = new List<Vector2>()
        {
            GameObject.Find("Customer (2)").transform.position,
            GameObject.Find("Customer (3)").transform.position,
            GameObject.Find("Customer (4)").transform.position,
            GameObject.Find("Customer (5)").transform.position,
            GameObject.Find("Customer (6)").transform.position,
            GameObject.Find("Customer (7)").transform.position,
            GameObject.Find("Customer (8)").transform.position,
            GameObject.Find("Customer (9)").transform.position,
        };
        DeletePackagesFromMap();
        RefreshAvailablePackages();
    }

    public void Update()
    {
        moneyPrefab.GetComponent<TMP_Text>().text = ((int)money).ToString() + " $";
        speedometerPrefab.GetComponent<TMP_Text>().text = "Speed: " + ((int)Driver.moveSpeed).ToString() + " km / h";
        fuelEfficiencyPrefab.GetComponent<TMP_Text>().text = "Mileage: " + ((fuelIntensity * 3000)).ToString("0.00") + " l / 100 km";
        storagePrefab.GetComponent<TMP_Text>().text = "Storage: " + (maxPackages).ToString() + " boxes";

    }

    private void DeletePackagesFromMap()
    {
        if (GameObject.FindGameObjectWithTag("Package") != null)
        {
            var packages = GameObject.FindGameObjectsWithTag("Package");
            foreach (var x in packages)
            {
                DestroyImmediate(x);
            }
        }
        for (int i = 1; i < 25; i++)
        {
            DestroyImmediate(GameObject.Find("Package (" + i + ")"));
        }

        for (int i = 2; i < 10; i++)
        {
            DestroyImmediate(GameObject.Find("Customer (" + i + ")"));

        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Depo" && !DeliveryState && !TouchingDepo)
        {
            OnDepoEnter();
        }
        if (collision.collider.tag == "UpgradeCenter")
        {
            OnUpgradeCenterEnter();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Depo")
        {
            OnDepoExit();
        }
        if (collision.collider.tag == "UpgradeCenter")
        {
            OnUpgradeCenterLeave();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Package" && currentRound.Size() < maxPackages && DeliveryState == false)
        {
            TouchingDepo = false;
            hasPackage = true;
            spriteRenderer.color = hasPackageColor;
            // Add the picked up package to the list of packages
            currentRound.AddPackage(collision.gameObject.GetComponent<Package>());
            // Destroy the package object
            Destroy(collision.gameObject);
        }
        else if (DeliveryState)
        {

            if (collision.CompareTag("Customer"))
            {
                var packageToDeliver = GetCompatiblePackage(collision);
                if (packageToDeliver.DeliveryLocation != null)
                {
                    currentRound.DeliverPackage(packageToDeliver);
                    currentRoundMultiplier += 0.1f;
                    Destroy(collision.gameObject);
                    Debug.Log("succesfull delivery");
                    addMoney(20);
                }
                if (currentRound.Packages.Count == 0)
                {
                    spriteRenderer.color = noPackageColor;
                    DeliveryState = false;
                    Debug.Log("succesfull delivery round");
                    addMoney((int)(10 * currentRoundMultiplier));
                    RefreshAvailablePackages();
                }
            }

            if (currentRound.Size() == 0) hasPackage = false;

        }

    }
    public void CreatePackageRound()
    {
        // Create a new package round with all collected packages
        PackageRound packageRound = new PackageRound();
        foreach (Package package in currentRound.Packages)
        {
            packageRound.AddPackage(package);
        }

        int numPackages = packageRound.Packages.Count;
        for (int i = 0; i < numPackages; i++)
        {
            Vector2 deliveryLocation = deliveryLocations[i % deliveryLocations.Count];
            packageRound.Packages[i].DeliveryLocation = deliveryLocation;
        }

        packageRounds.Add(packageRound);
        ComplexityCalculation.CalculateRoadComplexity(packageRound);
        ScrollBarPopulate.AddToList(packageRound);
        // Clear the current package round and refresh available packages
        currentRound = new PackageRound();
    }

    public void RefreshAvailablePackages()
    {
        // Destroy existing packages on the map
        DeletePackagesFromMap();

        ShuffleArray(pickupLocations);
        int numPackages = UnityEngine.Random.Range(minPickups, maxPickups + 1);
        // Instantiate new packages at random pickup locations
        for (int i = 0; i < numPackages; i++)
        {
            Vector2 pickupLocation = pickupLocations[i % pickupLocations.Count];
            Instantiate(packagePrefab, pickupLocation, Quaternion.identity);
        }
        Driver.canMove = true;
        ScrollBarPopulate.GUI.SetActive(false);
    }

    public void SelectPackageRound(PackageRound packageRound)
    {
        // Destroy existing packages on the map
        currentRound.Destroy();
        DeletePackagesFromMap();


        ShuffleArray(pickupLocations);
        int numPackages = packageRound.Packages.Count;
        for (int i = 0; i < numPackages; i++)
        {
            Instantiate(customerPrefab, packageRound.Packages[i].DeliveryLocation, Quaternion.identity);
        }

        // Set the current package round
        currentRound = packageRound;
        spriteRenderer.color = hasPackageColor;
        DeliveryState = true;
        Destroy(packageRound.gameObject);
        Driver.canMove = true;
        ScrollBarPopulate.GUI.SetActive(false);
    }

    private void ShuffleArray(List<Vector2> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = list[j];
            list[j] = list[i];
            list[i] = temp;
        }
    }

    private Package GetCompatiblePackage(Collider2D customer)
    {
        foreach (var x in currentRound.Packages)
        {
            if (VectorCalculations.CheckIfAiscloseToB(x.DeliveryLocation, customer.transform))
            {
                return x;
            }
        }
        return null;
    }

    private void addMoney(int amount)
    {
        money += amount;
    }

    private static void textColorTransition()
    {
        if (fuel >= 75f)
        {
            fuelPrefab.GetComponent<TMP_Text>().color = fullFuel;

        }
        if (fuel < 75f)
        {
            fuelPrefab.GetComponent<TMP_Text>().color = mediumFuel;
        }

        if (fuel < 30f)
        {
            fuelPrefab.GetComponent<TMP_Text>().color = lowFuel;
        }
    }
    public static void DeductFuel()
    {
        fuel -= fuelIntensity;

        textColorTransition();

        if (fuel < 0)
        {
            fuel = 0;
            //todo: implement gradual slowing.
            Driver.setMoveSpeed(1);
        }
        fuelPrefab.GetComponent<TMP_Text>().text = "FUEL: " + ((int)fuel).ToString() + " %";
    }
    public void RefillFuel()
    {
        float missingFuel = 100 - fuel;
        if (money >= (int)(missingFuel * 0.6f))
        {
            money -= (int)(missingFuel * 0.6f);
            fuel = 100;
            fuelPrefab.GetComponent<TMP_Text>().color = fullFuel;
            fuelPrefab.GetComponent<TMP_Text>().text = fuel.ToString() + " %";
            fuelRefillPrefab.GetComponent<TMP_Text>().text = "REFILL FOR: 0 $";
        }
    }

    private void OnDepoEnter()
    {

        ScrollBarPopulate.GUI.SetActive(true);
        ScrollBarPopulate.listOfUpgrades.SetActive(false);
        ScrollBarPopulate.listOfPackages.SetActive(true);

        TouchingDepo = true;
        if (currentRound.Size() != 0) CreatePackageRound();
        spriteRenderer.color = noPackageColor;
        fuelRefillPrefab.GetComponent<TMP_Text>().text = "REFILL FOR: " + (int)((100 - fuel) * 0.6f) + " $";
    }

    private void OnDepoExit()
    {
        TouchingDepo = false;
        ScrollBarPopulate.GUI.SetActive(false);
    }

    private void OnUpgradeCenterEnter()
    {
        ScrollBarPopulate.GUI.SetActive(true);
        ScrollBarPopulate.listOfUpgrades.SetActive(true);
        ScrollBarPopulate.listOfPackages.SetActive(false);
    }

    private void OnUpgradeCenterLeave()
    {
        ScrollBarPopulate.GUI.SetActive(false);
    }

    public void UpgradeStorage()
    {
        if (maxPackages < 20)
        {
            if (money >= 100)
            {
                money -= 100;
                maxPackages += 1;
            }
        }
    }

    public void UpgradeSpeed()
    {
        if (Driver.moveSpeed < 50)
        {
            if (money >= 100)
            {
                money -= 100;
                Driver.setMoveSpeed(Driver.moveSpeed + 1);
            }
        }
    }

    public void UpgradeFuel()
    {
        if (fuelIntensity > 0.001)
            if (money >= 250)
            {
                money -= 100;
                fuelIntensity -= 0.00001f;
            }
    }

}
