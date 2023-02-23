using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulateScrollView : MonoBehaviour
{
    public GameObject prefab; // reference to the prefab that will be instantiated
    public int numberOfItems = 20; // number of items to display in the scroll view
    public RectTransform content; // reference to the content transform of the scroll view
    private void Start()
    {
        // calculate the height of each item based on the size of the prefab
        float itemHeight = prefab.GetComponent<RectTransform>().rect.height;

        // calculate the height of the content based on the number of items and the height of each item

        // set the size of the content to match the calculated height

        // instantiate the prefabs and set their parent to the content
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject newItem = Instantiate(prefab, content);
            newItem.GetComponentInChildren<TMP_Text>().text = "Item " + i.ToString();

            // set the position of the new item based on the height of the previous items
            float y = -(i * itemHeight);
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        }

    }
    void Update()
    {
        //if player is near the depo.
        
    }
}
