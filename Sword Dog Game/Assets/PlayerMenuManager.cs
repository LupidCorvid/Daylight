using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{

    public List<GameObject> menus = new List<GameObject>();
    private int currentMenuID;
    public int currentMenu
    {
        get
        {
            return currentMenuID;
        }
        set
        {
            currentMenuID = value % menus.Count;
            if (currentMenuID < 0)
                currentMenuID += menus.Count;
        }
    }

    public GameObject slideIn;
    public GameObject slideOut;
    public bool fromLeft;

    bool transitioning = false;

    public float slideSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            float offsetAmount = 1000;
            if (fromLeft)
                offsetAmount *= -1;
            Vector3 outMoveDist = slideOut.transform.position - new Vector3(offsetAmount + Screen.width / 2, Screen.height / 2, 0);
            if (outMoveDist.magnitude <= 1)
                outMoveDist = outMoveDist * 1;
            slideOut.transform.position -= (outMoveDist * Time.deltaTime * slideSpeed);

            Vector3 inMoveDist = slideIn.transform.position - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (inMoveDist.magnitude <= 1)
                inMoveDist = outMoveDist * 1;

            slideIn.transform.position -= inMoveDist * Time.deltaTime * slideSpeed;

            if(Mathf.Abs((slideIn.transform.position - new Vector3(Screen.width / 2, Screen.height / 2, 0)).x) < 1)
            {
                slideIn.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                slideOut.transform.position = new Vector3(offsetAmount + Screen.width / 2, Screen.height / 2, 0);
                transitioning = false;
                if (fromLeft)
                    currentMenu++;
                else
                    currentMenu--;
            }
        }
    }

    //Items slide in to the right from the left;
    public void PageLeft()
    {

        slideOut = menus[currentMenu];
        slideIn = menus[(currentMenu + 1) % menus.Count];
        //if(!transitioning)
        slideIn.transform.position = new Vector3(-1000 + Screen.width / 2, Screen.height / 2, 0);
        fromLeft = false;
        transitioning = true;
        
    }

    //Items slide in to the left from the right
    public void PageRight()
    {

        if (transitioning)
            return;
        slideOut = menus[currentMenu];
        if (currentMenu - 1 >= 0)
            slideIn = menus[(currentMenu - 1) % menus.Count];
        else
            slideIn = menus[^1];
        //if (!transitioning)
        slideIn.transform.position = new Vector3(1000 + Screen.width / 2, Screen.height / 2, 0);
        fromLeft = true;
        transitioning = true;
    }
}
