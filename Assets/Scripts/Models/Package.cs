using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    public GameObject PackageObject;
    public Vector3 DeliveryLocation;

    public Package(GameObject packageObject)
    {
        PackageObject = packageObject;
        DeliveryLocation = new Vector3(0,0,0);
    }
}
