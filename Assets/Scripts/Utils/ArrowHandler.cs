using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{
    public static GameObject arrowPrefab;

    private Vector2 nearestPosition;
    private int nearestIndex = -1;
    public static List<Vector2> targetPositions = new List<Vector2>();
    private static RectTransform triangleRect;
    void Start()
    {
        arrowPrefab = GameObject.Find("Arrow");
        triangleRect = arrowPrefab.GetComponent<RectTransform>();
        triangleRect.anchorMin = new Vector2(0.5f, 0.5f);
        triangleRect.anchorMax = new Vector2(0.5f, 0.5f);
        triangleRect.pivot = new Vector2(0.5f, 0.5f);
        triangleRect.anchoredPosition = Vector2.zero;
        arrowPrefab.SetActive(DeliveryManager.hasNavigation);
    }

    void Update()
    {
        if (arrowPrefab.activeSelf)
        {
            float nearestDistance = Mathf.Infinity;
            for (int i = 0; i < targetPositions.Count; i++)
            {
                float distance = Vector2.Distance(Driver.currentPosition, targetPositions[i]);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPosition = targetPositions[i];
                    nearestIndex = i;
                }
            }
            if (arrowPrefab != null && nearestPosition != null)
            {
                arrowPrefab.transform.position = Driver.currentPosition;

                if (nearestIndex >= 0)
                {
                    Vector3 nearestPos3D = nearestPosition;
                    Vector3 dir = nearestPos3D - arrowPrefab.transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    arrowPrefab.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
            if (nearestDistance < 0.5f)
            {
                targetPositions.RemoveAt(nearestIndex);
                nearestIndex = -1;

                // Check if there are any positions left
                if (targetPositions.Count == 0)
                {
                    Destroy(arrowPrefab);
                }
            }
        }
    }
}
