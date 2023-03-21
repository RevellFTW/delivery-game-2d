using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PackageHandler
    {
        public static List<Vector2> ConvertPackagesToCoordinates(List<Package> packages)
        {
            List<Vector2> coordinates = new List<Vector2>();

            // Loop through each package in the list and extract the Vector2 attribute
            foreach (Package package in packages)
            {
                Vector2 vector2 = package.DeliveryLocation;
                coordinates.Add(vector2);
            }

            return coordinates;
        }
    }
}
