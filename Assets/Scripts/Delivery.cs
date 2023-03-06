using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] Color32 hasPackageColor = new Color32(1, 1, 1, 1);
    [SerializeField] Color32 noPackageColor = new Color32(0, 1, 0, 1);


    public int maxPackages = 5; // Maximum number of packages that the player can carry at a time
    public bool hasPackage = false; // Whether the player is currently carrying a package
    public List<Package> depoPackages = new List<Package>();

    private SpriteRenderer spriteRenderer; // Reference to the player's sprite renderer component

    #region prefabs

    public GameObject packagePrefab;
    [SerializeField]
    public static List<PackageRound> packageRounds;
    #endregion


    #region Pickup And Delivery Locations
    public List<Vector2> pickupLocations;
    public List<Vector2> deliveryLocations;
    #endregion

    #region game state
    [SerializeField]
    public bool DeliveryState = false;
    #endregion
    [SerializeField]
    private static PackageRound currentRound;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            GameObject.Find("Package (17)").transform.position
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
    }

    public void Update()
    {
        Debug.Log("packageround size: " + packageRounds.Count);
        Debug.LogWarning("cur round size:" + currentRound.Size());
        if (CheckIfAiscloseToB(spriteRenderer.transform, GameObject.Find("Depo").transform) && DeliveryState == false)
        {
            //TODO: add more if statements because it generates too much packages
            if (currentRound.Size() != 0) CreatePackageRound();
            spriteRenderer.color = noPackageColor;
        }
    }

    private void DeletePackagesFromMap()
    {
        for (int i = 1; i < 18; i++)
        {
            Destroy(GameObject.Find("Package (" + i + ")"));
            Debug.Log("Packages deleted");
        }

        for (int i = 2; i < 9; i++)
        {
            Destroy(GameObject.Find("Customer (" + i + ")"));
            Debug.Log("Customers deleted");

        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Ouch");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Package" && currentRound.Size() < maxPackages && DeliveryState == false)
        {

            hasPackage = true;
            spriteRenderer.color = hasPackageColor;
            Debug.Log("Package picked up");
            // Add the picked up package to the list of packages
            currentRound.AddPackage(other.gameObject.GetComponent<Package>());
            // Destroy the package object
            Destroy(other.gameObject);
        }
        else if (DeliveryState)
        {

            if (other.CompareTag("Customer") && CheckIfAiscloseToB(currentRound.Packages[0].transform, other.transform))
            {
                currentRound.DeliverPackage(currentRound.Packages[0]);
            }

            if (currentRound.Size() == 0) hasPackage = false;

        }

    }

    private bool CheckIfAiscloseToB(Transform a, Transform b)
    {
        //check if package is close to costumer
        if (Vector2.Distance(a.position, b.position) < 9.5f)
        {
            return true;
        }
        else
        {
            return false;
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
        packageRounds.Add(packageRound);

        // Clear the current package round and refresh available packages
        currentRound = new PackageRound();
        RefreshAvailablePackages();
        Debug.Log("Package round created");
    }

    public void RefreshAvailablePackages()
    {
        // Destroy existing packages on the map
        currentRound.Destroy();

        // Instantiate new packages at random pickup locations
        foreach (Vector2 pickupLocation in pickupLocations)
        {
            GameObject packageObject = Instantiate(packagePrefab, pickupLocation, Quaternion.identity);
            Package package = new Package(packageObject);
            PackageRound packageRound = new PackageRound();
            packageRound.AddPackage(package);
            packageRounds.Add(packageRound);
        }
    }

    //TODO: Make UI list click execute this.
    public void SelectPackageRound(PackageRound packageRound)
    {
        // Destroy existing packages on the map
        currentRound.Destroy();

        // Load the packages onto the map
        foreach (Package package in packageRound.Packages)
        {
            // Set the delivery location for the package
            int randomIndex = UnityEngine.Random.Range(0, deliveryLocations.Count);
            Vector2 deliveryLocation = deliveryLocations[randomIndex];
            package.DeliveryLocation = deliveryLocation;

            // Create a delivery location marker on the map
            GameObject deliveryLocationMarker = new GameObject("Package");
            deliveryLocationMarker.tag = "Package";
            deliveryLocationMarker.transform.position = deliveryLocation;
            //deliveryLocationMarker.transform.SetParent(map);
        }

        // Set the current package round
        currentRound = packageRound;
    }

}
