using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectPrompt : MonoBehaviour
{
    public GameObject useButton;
    public GameObject infoButton;
    public GameObject discardButton;
    public GameObject cancelButton;
    public GameObject description;
    public GameObject backButton;

    public TMPro.TextMeshProUGUI descriptionText;

    public Item currentItem;

    public ItemListing fromListing;

    public void Start()
    {
        close();
    }

    public void close()
    {
        transform.position = new Vector3(-5000, -5000, -5000);
        //if(fromListing?.bgSprite != null)
        //    fromListing.bgSprite.color = new Color32(195, 149, 14, 255);
        if (fromListing != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(fromListing.gameObject);
        
    }

    public void useItem()
    {
        if(currentItem?.quantity > 0)
            currentItem?.OnUse(PlayerMenuManager.main.player);
        if (currentItem?.quantity <= 0)
            close();
    }

    public void openDefault()
    {
        ChangeFont();
        useButton.SetActive(true);
        infoButton.SetActive(true);
        discardButton.SetActive(true);
        cancelButton.SetActive(true);
        description.SetActive(false);
        backButton.SetActive(false);
    }

    public void ChangeFont()
    {
        useButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().font = FontManager.main.currFont;
        infoButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().font = FontManager.main.currFont;
        discardButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().font = FontManager.main.currFont;
        cancelButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().font = FontManager.main.currFont;
        
    }

    public void opendescription()
    {
        useButton.SetActive(false);
        infoButton.SetActive(false);
        discardButton.SetActive(false);
        cancelButton.SetActive(false);
        description.SetActive(true);
        backButton.SetActive(true);
        descriptionText.text = currentItem.description;
        descriptionText.font = FontManager.main.currFont;
        backButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().font = FontManager.main.currFont;
    }

    public void discard()
    {
        currentItem.quantity--;
    }

    public void selectItem(Item item, Vector3 position)
    {
        currentItem = item;
        transform.position = position;
        openDefault();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(useButton);
        //fromListing.bgSprite.color = Color.white;
    }
}
