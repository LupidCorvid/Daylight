using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class NamingScreenScript : MonoBehaviour
{
    public string inputtedName = "";

    public int cursorPos = 0;

    public List<TextMeshProUGUI> chars = new List<TextMeshProUGUI>();

    public GameObject NameDisplay;

    public GameObject cursor;

    public List<TextMeshProUGUI> selectableInputs = new List<TextMeshProUGUI>();

    public GameObject KeyListHolder;

    public GameObject initialSelectedObject;

    GameObject lastSelectedValidObject;

    bool loadingScene = false;

    // Start is called before the first frame update
    void Start()
    {
        chars.AddRange(NameDisplay.GetComponentsInChildren<TextMeshProUGUI>());

        for(int i = 0; i < chars.Count; i++)
        {
            if (chars[i].transform.childCount > 0)
                chars.RemoveAt(i);
        }

        //selectableInputs.AddRange(KeyListHolder.GetComponentsInChildren<TextMeshProUGUI>());
        EventSystem.current.SetSelectedGameObject(initialSelectedObject);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplayedName();

        //cursor.transform.position = selectableInputs[cursorPos].transform.position + Vector3.right * (selectableInputs[cursorPos].rectTransform.rect.width - 12.5f);
        //cursor.transform.GetChild(0).position = selectableInputs[cursorPos].rectTransform.rect.width
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            cursor.transform.position = EventSystem.current.currentSelectedGameObject.transform.position;
            //cursor.transform.GetChild(0).position = Vector3.right * EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.width;
            cursor.transform.GetChild(0).localPosition = Vector3.right * (EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.width/2 + 50);
            lastSelectedValidObject = EventSystem.current.currentSelectedGameObject;
        }
        else
            EventSystem.current.SetSelectedGameObject(lastSelectedValidObject);

        //if (InputReader.inputs.actions["Interact"].WasPressedThisFrame())
        //{
        //    TextMeshProUGUI hoveredItem = EventSystem.current.currentSelectedGameObject.GetComponent<TextMeshProUGUI>();
        //    if (hoveredItem == null)
        //        return;
        //    if (hoveredItem.text.Length <= 1)
        //        //inputtedName += selectableInputs[cursorPos].text;
        //        inputtedName += hoveredItem.text;

        //    else
        //    {
        //        switch (hoveredItem.text)
        //        {
        //            case "Backspace":
        //                if (inputtedName.Length > 0)
        //                    inputtedName = inputtedName.Substring(0, inputtedName.Length - 1);
        //                break;
        //            case "Accept":
        //                Debug.Log("Name accept hit");
        //                if (!DialogSource.stringVariables.ContainsKey(" playerName"))
        //                    DialogSource.stringVariables.Add(" playerName", inputtedName);
        //                else
        //                    DialogSource.stringVariables[" playerName"] = inputtedName;
        //                break;
        //        }
        //    }
        //}

        //if (InputReader.inputs.actions["Move"].WasPressedThisFrame())
        //{
        //    if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x < -.1f)
        //        cursorPos--;
        //    if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x > .1f)
        //        cursorPos++;
        //}

        //cursorPos = (cursorPos + selectableInputs.Count) % selectableInputs.Count;
        if (InputReader.inputs.actions["Interact"].WasPressedThisFrame())
        {
            ReceivedInput();
        }
    }

    public void UpdateDisplayedName()
    {
        for(int i = 0; i < chars.Count; i++)
        {
            if (i < inputtedName.Length)
                chars[i].text = "" + inputtedName[i];
            else
                chars[i].text = "";
        }
    }

    public void Close()
    {

    }

    public void ReceivedInput()
    {
       
        TextMeshProUGUI hoveredItem = EventSystem.current.currentSelectedGameObject.GetComponent<TextMeshProUGUI>();
        if (hoveredItem == null)
            return;
        if (hoveredItem.text.Length <= 1 && inputtedName.Length < chars.Count)
            //inputtedName += selectableInputs[cursorPos].text;
            inputtedName += hoveredItem.text;

        else
        {
            switch (hoveredItem.text)
            {
                case "Backspace":
                    if (inputtedName.Length > 0)
                        inputtedName = inputtedName.Substring(0, inputtedName.Length - 1);
                    break;
                case "Accept":
                    Debug.Log("Name accept hit");
                    if (loadingScene || inputtedName.Length <= 0)
                        break;
                    if (!DialogSource.stringVariables.ContainsKey(" playerName"))
                        DialogSource.stringVariables.Add(" playerName", inputtedName);
                    else
                        DialogSource.stringVariables[" playerName"] = inputtedName;

                    if (true)
                    {
                        ChangeScene.LoadScene("IntroCutscene", "", false);
                        loadingScene = true;
                    }
                    break;
            }
        }
        
    }

    
}
