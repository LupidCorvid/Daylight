using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public MenuManager manager;
    public virtual void OpenMenu()
    {
        Cursor.visible = true;
    }

    public virtual void CloseMenu()
    {
        Cursor.visible = false;
    }

    public virtual void selectDown()
    {

    }

    public virtual void selectUp()
    {

    }


}
