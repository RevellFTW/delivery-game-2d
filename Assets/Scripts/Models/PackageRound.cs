using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageRound : MonoBehaviour
{
    public List<Package> Packages = new List<Package>();

    public string complexity;

    public void AddPackage(Package package)
    {
        Packages.Add(package);
    }

    public void DeliverPackage(Package package)
    {
        Packages.Remove(package);
    }

    public void SetComplexity(float number)
    {
        this.complexity = ((int)number).ToString();
    }

    public string GetComplexity()
    {
        return "level " +  complexity + " complexity";
    }

    
    public void Destroy()
    {
        Packages.Clear();
    }

    public int Size()
    { 
        return Packages.Count; 
    }
}
