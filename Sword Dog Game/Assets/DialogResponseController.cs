using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogResponseController : MonoBehaviour
{
    public GameObject responsePrefab;
    public List<GameObject> options;
    public GameObject selectedObjectPointer;

    public RectTransform responsesContainer;

    public CanvasGroup canvasGroup;
    
    private int _selectedOption = 0;
    public int selectedOption
    {
        set
        {
            _selectedOption = value;
            chooseOption(_selectedOption);
        }
        get
        {
            return _selectedOption;
        }

    }

    public void addResponse(string text)
    {
        GameObject addedObject = Instantiate(responsePrefab, transform);
        options.Add(addedObject);
    }

    public void clearResponses()
    {
        for(int i = options.Count; i >= 0; i++)
        {
            Destroy(options[i]);
            options.RemoveAt(i);
        }
    }

    public event Action<int> optionChosen;

    public void setResponses(params string[] text)
    {
        clearResponses();
        for(int i = 0; i < text.Length; i++)
        {
            addResponse(text[i]);
        }
        open();
    }

    public void chooseOption(int option)
    {
        if (options.Count == 0 || options.Count <= 0)
            return;
        option = Mathf.Abs(option) % options.Count;

        Vector3 finalPosition = default;
        finalPosition.x = (-responsesContainer.rect.width + 20) + responsesContainer.position.x;
        finalPosition.y = options[option].transform.position.y;
        selectedObjectPointer.transform.position = finalPosition;
    }

    public void open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        selectedOption = 0;
    }

    public void close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedOption--;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedOption++;
        }
    }

}
