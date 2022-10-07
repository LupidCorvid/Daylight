using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogController : MonoBehaviour
{
    public static DialogController main;

    public CanvasGroup panel;
    public int panelAlpha = 255;

    public TextMeshProUGUI textDisplay;

    public DialogSource source;

    public bool reading = false;

    public DialogResponseController responseController;

    public string text
    {
        get
        {
            return textDisplay.text;
        }
        set
        {
            textDisplay.text = value;
        }
    }
    void Awake()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (reading)
            textDisplay.text = source.read();
    }
    public void openBox()
    {
        panel.alpha = panelAlpha;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
    public void closeBox()
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        responseController.close();
    }

    public void promptSelections(params string[] options)
    {
        responseController.setResponses(options);
        
    }
    public void receiveResponse(int response)
    {
        source.receiveResponse(response);
        responseController.close();
    }

    public void setSource(DialogSource newSource)
    {
        if(source != null)
            source.requestOptionsStart -= promptSelections;
        source = newSource;
        newSource.requestOptionsStart += promptSelections;
    }
}
