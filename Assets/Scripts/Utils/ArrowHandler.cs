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
        //triangleRect.anchorMin = new Vector2(0.5f, 1f);
        //triangleRect.anchorMax = new Vector2(0.5f, 1f);
        //triangleRect.pivot = new Vector2(0.5f, 1f);
        //triangleRect.anchoredPosition = new Vector2(0f, -triangleRect.sizeDelta.y / 2f);
        triangleRect.anchorMin = new Vector2(0.5f, 0.5f);
        triangleRect.anchorMax = new Vector2(0.5f, 0.5f);
        triangleRect.pivot = new Vector2(0.5f, 0.5f);
        triangleRect.anchoredPosition = Vector2.zero;
    }

    void Update()
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
        if (arrowPrefab != null)
        {
            arrowPrefab.transform.position = Driver.currentPosition;

            if (nearestIndex >= 0)
            {
                //Vector2 direction = (nearestPosition - Driver.currentPosition).normalized;
                //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                //arrowPrefab.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                Vector2 direction = nearestPosition - (Vector2)Driver.currentPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (direction.y < 0)
                {
                    angle += 180f;
                }
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
