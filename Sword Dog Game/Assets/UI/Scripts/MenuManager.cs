using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuManager : MonoBehaviour
{
    public BaseManager menu;
    public static MenuManager main;

    public static bool inMenu = false;

    public static Action menuClosed;

    public void Awake()
    {
        if (main == null)
            main = this;
    }

    public void closeMenu()
    {
        menu.CloseMenu();
        inMenu = false;
        menuClosed?.Invoke();
    }

    public void openMenu(GameObject menuToAdd)
    {
        GameObject addedMenu = Instantiate(menuToAdd, transform);
        menu = addedMenu.GetComponent<BaseManager>();
        inMenu = true;
        menu.manager = this;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //Handle controller inputs and other general menu interactions here?

    public void Update()
    {
        if (inMenu)
        {
            if (Input.GetKeyDown(KeyCode.W))
                menu.selectUp();
            if (Input.GetKeyDown(KeyCode.S))
                menu.selectDown();
        }
    }
}
