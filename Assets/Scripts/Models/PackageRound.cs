using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageRound : MonoBehaviour
{
    public List<Package> Packages = new List<Package>();

    public string complexity = "todo";

    public void AddPackage(Package package)
    {
        Packages.Add(package);
    }

    public void DeliverPackage(Package package)
    {
        Packages.Remove(package);
    }

    public int Size()

    { return Packages.Count; }

    
    public void Destroy()
    {
        Packages.Clear();
    }
}
