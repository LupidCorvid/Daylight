using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public MenuManager manager;
    public virtual void OpenMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public virtual void CloseMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public virtual void selectDown()
    {

    }

    public virtual void selectUp()
    {

    }


}
