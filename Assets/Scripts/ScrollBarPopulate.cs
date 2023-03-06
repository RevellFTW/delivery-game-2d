using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollBarPopulate : MonoBehaviour
{

    public GameObject prefab; // reference to the prefab that will be instantiated
    public RectTransform content; // reference to the content transform of the scroll view
    public int numberOfItems = 20; // number of items to display in the scroll view
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject newItem = Instantiate(prefab, content);
            newItem.GetComponentInChildren<TMP_Text>().text = "Item " + i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
