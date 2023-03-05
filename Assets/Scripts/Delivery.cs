using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] Color32 hasPackageColor = new Color32 (1,1,1,1);
    [SerializeField] Color32 noPackageColor = new Color32 (0,1,0,1);
    [SerializeField] float destoryDelay = 0.1f;


    public int maxPackages = 5; // Maximum number of packages that the player can carry at a time
    public bool hasPackage = false; // Whether the player is currently carrying a package
    public List<Package> packages = new List<Package>();
    public List<Package> depoPackages = new List<Package>();
    
    private int currentPackages = 0; // Current number of packages being carried by the player
    private SpriteRenderer spriteRenderer; // Reference to the player's sprite renderer component


    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    

    void OnCollisionEnter2D(Collision2D other) 
    {
        Debug.Log("Ouch");
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Package"  && packages.Count < maxPackages)
        {
            Debug.Log("Package picked up");
            hasPackage = true;
            spriteRenderer.color = hasPackageColor;

            // Add the picked up package to the list of packages
            packages.Add(other.gameObject.GetComponent<Package>());

            // Destroy the package object
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Customer"))
        {
            Debug.Log("Package delivered");

            // Deliver the package only if it was picked up from the depo
            if (hasPackage && packages[0].deliveryLocation != null)
            {

                spriteRenderer.color = noPackageColor;

                // Remove the delivered package from the list of packages
                packages.RemoveAt(0);
                if(packages.Count == 0) hasPackage = false;

            }
            else
            {
                Debug.Log("This package was not picked up from the depo.");
            }
        }
        else if (other.tag == "Depo")
        {
            Debug.Log("Loading packages into depo");

            // Load all packages from the delivery vehicle into the depo
            foreach (Package package in packages)
            {
                package.deliveryLocation = new Vector3();
                depoPackages.Add(package);
            }

            packages.Clear();
        }
    }
    
}
