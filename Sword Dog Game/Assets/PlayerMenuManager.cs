using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public static bool open = false;

    public CanvasGroup menusGroup;

    public CanvasGroup playerMenusGroup;

    public static PlayerMenuManager main;
    public Entity player;

    public bool cancelTransition = false;
    public bool skipTransition = false;

    public GameObject defaultButton;

    public float offsetAmount
    {
        get
        {
            return 1000 * Screen.width / 1092;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        //SceneHelper.FinishedChangeScene += FindPlayer;
        for(int i = 1; i < menus.Count; i++)
        {
            menus[i].transform.position += Vector3.right * offsetAmount;
        }
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.controller?.entityBase;

        if (main == null)
            main = this;
        //Problem is that slideIn != menus[currentMenu]. No longer hit because it avoids that only when things settle (which is when this is checked)
        if (!transitioning &&  slideIn != null && (slideIn != menus[currentMenu] || slideIn == slideOut))
            Debug.Log("EERRORRR" + (slideIn == slideOut) + " <- in == out, curr != in ->" + (slideIn != menus[currentMenu]));

        if(InputReader.inputs != null && InputReader.inputs.actions["Inventory"].WasPressedThisFrame())
        {
            if (!PauseScreen.paused && PauseScreen.canPause && !MenuManager.inMenu)
            {
                if (open)
                {
                    closeMenu();
                }
                else
                {
                    openMenu();
                }
            }
        }

        if(transitioning && open)
        {
            float offsetAmount = this.offsetAmount;
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
                //Fix for menu changing pages to itself, bug shows symptom of over thing to leaving for testing
                currentMenu = menus.IndexOf(slideIn);
            }
        }
    }

    public void openMenu()
    {
        open = true;
        menusGroup.alpha = 1;
        menusGroup.interactable = true;
        menusGroup.blocksRaycasts = true;

        playerMenusGroup.alpha = 1;
        playerMenusGroup.interactable = true;
        playerMenusGroup.blocksRaycasts = true;


        EventSystem.current.SetSelectedGameObject(defaultButton);
        CanvasManager.HideHUD(true);
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void closeMenu()
    {
        open = false;

        menusGroup.alpha = 0;
        menusGroup.interactable = false;
        menusGroup.blocksRaycasts = false;

        playerMenusGroup.alpha = 0;
        playerMenusGroup.interactable = false;
        playerMenusGroup.blocksRaycasts = false;


        InventoryManager.main.itemInfoPopup.close();

        CanvasManager.ShowHUD(true);
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Items slide in to the right from the left;
    public void PageLeft()
    {
        InventoryManager.main.itemInfoPopup.close();
        if (transitioning)
        {
            if (fromLeft)
            {
                currentMenu--;
                
            }
            else
            {
                //if (menus[(currentMenu + 1) % menus.Count] == slideOut)
                //    return;
                currentMenu++;
                if (Mathf.Abs((slideOut.transform.position - new Vector3(offsetAmount + Screen.width / 2, Screen.height / 2, 0)).x) < 250 * Screen.width/ 1092)
                    skipTransition = true;
                else
                {
                    currentMenu++;
                    return;
                }

            }
        }

        //if (transitioning)
        //{
        //    if (fromLeft)
        //        currentMenu--;
        //    else
        //        currentMenu++;
        //}



        slideOut = menus[currentMenu];
        slideIn = menus[(currentMenu + 1) % menus.Count];
        if (!transitioning || skipTransition)
            slideIn.transform.position = new Vector3(-offsetAmount + Screen.width / 2, Screen.height / 2, 0);
        fromLeft = false;
        transitioning = true;
        skipTransition = false;
    }

    //Items slide in to the left from the right
    public void PageRight()
    {

        //if (transitioning)
        //    return;
        InventoryManager.main.itemInfoPopup.close();
        if (transitioning)
        {
            if (fromLeft)
            {
                currentMenu--;
                if (Mathf.Abs((slideOut.transform.position - new Vector3(-offsetAmount + Screen.width / 2, Screen.height / 2, 0)).x) < 250 * Screen.width / 1092)
                    //slideIn.transform.position = new Vector3(1000 + Screen.width / 2, Screen.height / 2, 0);
                    skipTransition = true;
                else
                {
                    currentMenu++;
                    return;
                }
            }
            else
            {
                currentMenu++;
                //skipTransition = true;

            }
        }
        //else
        //    skipTransition = false;
        //if (transitioning && !(skipTransition && Mathf.Abs((slideOut.transform.position - new Vector3(1000 + Screen.width / 2, Screen.height / 2, 0)).x) < 250))
        //    return;

        slideOut = menus[currentMenu];
        if (currentMenu - 1 >= 0)
            slideIn = menus[(currentMenu - 1) % menus.Count];
        else
            slideIn = menus[^1];
        if (!transitioning || skipTransition)
            slideIn.transform.position = new Vector3(offsetAmount + Screen.width / 2, Screen.height / 2, 0);
        fromLeft = true;
        transitioning = true;
        skipTransition = false;
    }
}
