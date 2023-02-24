using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public BaseManager menu;
    
    public void closeMenu()
    {
        menu.CloseMenu();
    }

    public void openMenu(GameObject menuToAdd)
    {
        GameObject addedMenu = Instantiate(menuToAdd, transform);
        menu = addedMenu.GetComponent<BaseManager>();
    }

    //Handle controller inputs and other general menu interactions here?
}
