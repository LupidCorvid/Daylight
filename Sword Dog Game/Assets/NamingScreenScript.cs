using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        chars.AddRange(NameDisplay.GetComponentsInChildren<TextMeshProUGUI>());

        for(int i = 0; i < chars.Count; i++)
        {
            if (chars[i].transform.childCount > 0)
                chars.RemoveAt(i);
        }

        selectableInputs.AddRange(KeyListHolder.GetComponentsInChildren<TextMeshProUGUI>());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplayedName();

        cursor.transform.position = selectableInputs[cursorPos].transform.position + Vector3.right * (selectableInputs[cursorPos].rectTransform.rect.width - 12.5f);
        //cursor.transform.GetChild(0).position = selectableInputs[cursorPos].rectTransform.rect.width


        if (InputReader.inputs.actions["Interact"].WasPressedThisFrame())
        {
            if(selectableInputs[cursorPos].text.Length <= 1)
                inputtedName += selectableInputs[cursorPos].text;
            else
            {
                switch(selectableInputs[cursorPos].text)
                {
                    case "Backspace":
                        inputtedName = inputtedName.Substring(0, inputtedName.Length - 1);
                        break;
                    case "Accept":
                        Debug.Log("Name accept hit");
                        break;
                }
            }
        }

        if (InputReader.inputs.actions["Move"].WasPressedThisFrame())
        {
            if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x < -.1f)
                cursorPos--;
            if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x > .1f)
                cursorPos++;
        }

        cursorPos = (cursorPos + selectableInputs.Count) % selectableInputs.Count;

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
}
