using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Inventory currInventory = new Inventory();
    public GameObject itemListingPrefab;
    public GameObject listingHolder;

    public void refreshInventory()
    {
        for(int i = 0; i < listingHolder.transform.childCount; i++)
        {
            Destroy(listingHolder.transform.GetChild(i));
        }

        for(int i = 0; i < currInventory.contents.Count; i++)
        {
            Instantiate(itemListingPrefab, listingHolder.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
