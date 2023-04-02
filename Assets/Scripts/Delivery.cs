using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

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
    private int defaultMoveSpeed = 15;
    private int defaultMaxCargoSize = 20;
    #region cars
    public Sprite TruckSprite;
    public Sprite SportsCarSprite;
    public Sprite StarterCarSprite;
    #endregion
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
    public static PackageRound currentRound;
    [SerializeField]
    private static float currentRoundMultiplier = 1;
    public static bool hasNavigation;
    private float speedCost;
    private float storageCost;
    private float fuelCost;
    private string currentCar;
    private bool hasFasterCar;
    private bool hasTruck;
    private int speedUpgrade = 5;
    private int storageUpgrade = 10;

    void Start()
    {
        #region initialization
        hasTruck = false;
        hasFasterCar = false;
        currentCar = "starter";
        spriteRenderer = GetComponent<SpriteRenderer>();
        fuelPrefab = GameObject.Find("Fuel");
        storagePrefab = GameObject.Find("Storage");
        fuelEfficiencyPrefab = GameObject.Find("Mileage");
        speedometerPrefab = GameObject.Find("Speedometer");
        fuel = 100;
        fuelIntensity = 0.003f;
        currentRound = new PackageRound();
        hasNavigation = false;
        speedCost = 100;
        storageCost = 200;
        fuelCost = 250;

        #endregion
        packageRounds = new List<PackageRound>(){
            new PackageRound() };

        //TODO: REFACTOR IF EVERYTHING ELSE WORKS
        pickupLocations = new List<Vector2>();
        for (int i = 0; i < 40; i++)
        {
            if (GameObject.Find("Package (" + i + ")") != null)
            {
                pickupLocations.Add(GameObject.Find("Package (" + i + ")").transform.position);
            }
        }

        deliveryLocations = new List<Vector2>();
        for (int i = 1; i < 40; i++)
        {
            if (GameObject.Find("Customer (" + i + ")") != null)
            {
                deliveryLocations.Add(GameObject.Find("Customer (" + i + ")").transform.position);
            }
        }
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
        for (int i = 1; i < 40; i++)
        {
            DestroyImmediate(GameObject.Find("Package (" + i + ")"));
        }

        for (int i = 1; i < 40; i++)
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
                    ArrowHandler.arrowPrefab.SetActive(false);
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
        System.Random random = new System.Random();
        for (int i = 0; i < numPackages; i++)
        {
            Vector2 deliveryLocation = deliveryLocations[random.Next(deliveryLocations.Count)];
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
        System.Random rng = new System.Random();

        var shuffledPackages = packageRound.Packages.OrderBy(a => rng.Next()).ToList();

        int numPackages = packageRound.Packages.Count;
        for (int i = 0; i < numPackages; i++)
        {
            Instantiate(customerPrefab, shuffledPackages[i].DeliveryLocation, Quaternion.identity);
        }

        // Set the current package round
        currentRound = packageRound;
        if (hasNavigation)
        {
            ArrowHandler.arrowPrefab.SetActive(true);
            ArrowHandler.targetPositions = PackageHandler.ConvertPackagesToCoordinates(currentRound.Packages);
        }
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
        ScrollBarPopulate.scroll.GetComponent<ScrollRect>().content = ScrollBarPopulate.upgradeContent;
    }

    private void OnUpgradeCenterLeave()
    {
        ScrollBarPopulate.scroll.GetComponent<ScrollRect>().content = ScrollBarPopulate.content;
        ScrollBarPopulate.GUI.SetActive(false);

    }

    public void UpgradeStorage()
    {
        if (maxPackages < Driver.cargoLimit)
        {
            if (money >= storageCost)
            {
                money -= storageCost;
                storageCost += 50;
                maxPackages += 1;
                Driver.upgradedStorage = maxPackages;
            }
        }
    }

    public void UpgradeSpeed()
    {
        if (Driver.moveSpeed < Driver.speedLimit)
        {
            if (money >= speedCost)
            {
                money -= speedCost;
                speedCost += 50;
                Driver.upgradedSpeed = Driver.moveSpeed + 1;
                Driver.setMoveSpeed(Driver.moveSpeed + 1);
            }
        }
    }

    public void UpgradeFuel()
    {
        if (fuelIntensity > 0.001)
            if (money >= fuelCost)
            {
                money -= fuelCost;
                fuelCost += 50;
                fuelIntensity -= 0.00001f;
            }
    }
    public void BuyNavigation()
    {
        if (money >= 5000)
        {
            money -= 5000;
            hasNavigation = true;
        }
    }

    public void BuyTruck()
    {
        //swap out prefab sprite image property.
        //if (money >= 5000)
        //{
        //    if (currentCar != "truck")
        //    {
        //        Driver.cargoLimit += 10;
        //        currentCar = "truck";
        //    }
        //}
        if (money >= 5000 || hasTruck)
        {
            if (currentCar != "truck")
            {
                if (currentCar == "sportcar")
                {
                    if(Driver.moveSpeed > defaultMoveSpeed)
                    {
                        Driver.moveSpeed = defaultMoveSpeed;
                    }
                    Driver.speedLimit -= speedUpgrade;
                }
                Driver.cargoLimit += storageUpgrade;
                currentCar = "truck";
                if (!hasTruck)
                {
                    money -= 5000;
                    hasTruck = true;
                }
                if(Driver.upgradedStorage > maxPackages) maxPackages = Driver.upgradedStorage;
                spriteRenderer.sprite = TruckSprite;
            }
        }
    }

    public void BuyFasterCar()
    {
        if (money >= 5000 || hasFasterCar)
        {
            if (currentCar != "sportcar")
            {
                if(currentCar == "truck")
                {
                    if(maxPackages > defaultMaxCargoSize) { maxPackages = defaultMaxCargoSize; }
                    Driver.cargoLimit -= storageUpgrade;
                }
                Driver.speedLimit += speedUpgrade;
                currentCar = "sportcar";
                if (!hasFasterCar)
                {
                    money -= 5000;
                    hasFasterCar = true;
                }
                if(Driver.upgradedSpeed > Driver.moveSpeed) Driver.moveSpeed = Driver.upgradedSpeed;
                spriteRenderer.sprite = SportsCarSprite;
                
            }

        }
    }

}
