using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComplexityCalculation : MonoBehaviour
{
    public static void CalculateRoadComplexity(PackageRound packageround)
    {
        float complexityCounter = 0f;
        float maxComplexity = float.MinValue;
        float minComplexity = float.MaxValue;
        
        List<Vector2> path = PackageHandler.ConvertPackagesToCoordinates(packageround.Packages);

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 p1 = path[i];
            Vector2 p2 = path[i + 1];
            float distance = Vector2.Distance(p1, p2);
            float angle = Vector2.Angle(p2 - p1, Vector2.right);

            float complexity = 0.5f * distance + 0.5f * (180f - angle);
            complexityCounter += complexity;

            if (complexity > maxComplexity)
            {
                maxComplexity = complexity;
            }

            if (complexity < minComplexity)
            {
                minComplexity = complexity;
            }
        }
        if(path.Count <= 2)
        {
            packageround.SetComplexity(0);
            return;
        }
        float normalizedComplexity = (complexityCounter - minComplexity) / (maxComplexity - minComplexity);
        packageround.SetComplexity(normalizedComplexity);
    }
}
