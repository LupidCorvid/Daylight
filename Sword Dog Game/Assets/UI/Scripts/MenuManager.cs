using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public BaseManager menu;
    public static MenuManager main;

    public static bool inMenu = false;

    public void Awake()
    {
        if (main == null)
            main = this;
    }

    public void closeMenu()
    {
        menu.CloseMenu();
        inMenu = false;
    }

    public void openMenu(GameObject menuToAdd)
    {
        GameObject addedMenu = Instantiate(menuToAdd, transform);
        menu = addedMenu.GetComponent<BaseManager>();
        inMenu = true;
    }

    //Handle controller inputs and other general menu interactions here?

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            menu.selectUp();
        if (Input.GetKeyDown(KeyCode.S))
            menu.selectDown();

    }
}
