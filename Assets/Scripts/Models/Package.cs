using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    public Vector2 DeliveryLocation;
    public Package(GameObject packageObject)
    {
        DeliveryLocation = new Vector2(0,0);
    }
}
