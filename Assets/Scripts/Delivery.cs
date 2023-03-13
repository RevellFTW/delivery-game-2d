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

    #region prefabs & packages

    public GameObject packagePrefab;
    public GameObject customerPrefab;
    [SerializeField]
    public static List<PackageRound> packageRounds;
    private int minPickups = 5;
    private int maxPickups = 10;
    #endregion


    #region Pickup And Delivery Locations
    public List<Vector2> pickupLocations;
    public List<Vector2> deliveryLocations;
    #endregion

    #region game state
    [SerializeField]
    public bool DeliveryState = false;
    public bool TouchingDepo = false;
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
    }

    private void DeletePackagesFromMap()
    {
        if (GameObject.FindGameObjectWithTag("Package") != null)
        {
            var packages = GameObject.FindGameObjectsWithTag("Package");
            foreach(var x in packages)
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


    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.collider.tag == "Depo" && !DeliveryState && !TouchingDepo)
        {
            TouchingDepo = true;
            if (currentRound.Size() != 0) CreatePackageRound();
            spriteRenderer.color = noPackageColor;

            Driver.canMove = false;
            ScrollBarPopulate.GUI.SetActive(true);
        }

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Package" && currentRound.Size() < maxPackages && DeliveryState == false)
        {
            TouchingDepo = false;
            hasPackage = true;
            spriteRenderer.color = hasPackageColor;
            // Add the picked up package to the list of packages
            currentRound.AddPackage(other.gameObject.GetComponent<Package>());
            // Destroy the package object
            Destroy(other.gameObject);
        }
        else if (DeliveryState)
        {
            
            if (other.CompareTag("Customer"))
            {
                var packageToDeliver = GetCompatiblePackage(other);
                if (packageToDeliver.DeliveryLocation != null)
                {
                    currentRound.DeliverPackage(packageToDeliver);
                    Destroy(other.gameObject);
                    Debug.Log("succesfull delivery");
                }
                if(currentRound.Packages.Count == 0)
                {
                    spriteRenderer.color = noPackageColor;
                    DeliveryState = false;
                    Debug.Log("succesfull delivery round");

                }
            }

            if (currentRound.Size() == 0) hasPackage = false;

        }

    }

    private bool CheckIfAiscloseToB(Vector2 a, Transform b)
    {
        //check if package is close to costumer
        if (Vector2.Distance(a, new Vector2(b.position.x, b.position.y)) < 9.5f)
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
        ScrollBarPopulate.AddToList(currentRound);
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

    //TODO: Make UI list click execute this.
    public void SelectPackageRound(PackageRound packageRound)
    {
        // Destroy existing packages on the map
        currentRound.Destroy();
        DeletePackagesFromMap();


        ShuffleArray(pickupLocations);
        int numPackages = packageRound.Packages.Count;
        for (int i = 0; i < numPackages; i++)
        {
            Vector2 deliveryLocation = deliveryLocations[i % deliveryLocations.Count];
            packageRound.Packages[i].DeliveryLocation = deliveryLocation;
            Instantiate(customerPrefab, deliveryLocation, Quaternion.identity);
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
            if (CheckIfAiscloseToB(x.DeliveryLocation, customer.transform))
            {
                return x;
            }
        }
        return null;
    }
    
}
