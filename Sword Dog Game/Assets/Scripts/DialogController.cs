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

    public TextMeshProUGUI headerDisplay;

    public DialogSource source;

    public bool reading = false;

    public DialogResponseController responseController;

    public Animator anim;

    public bool readWhenOpen = false;

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
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Return))
            pauseWaitForInputEnd();
        if (reading)
        {
            textDisplay.text = source.read();
        }
    }
    public void finishOpen()
    {
        //panel.alpha = panelAlpha;
        panel.interactable = true;
        panel.blocksRaycasts = true;
        if (readWhenOpen)
            reading = true;
    }
    public void openBox()
    {
        CanvasManager.HideHUD();
        anim.SetBool("requestClose", false);
        panel.alpha = panelAlpha;
        textDisplay.alpha = 255;
        headerDisplay.alpha = 255;
    }
    public void closeBox()
    {
        anim.SetBool("requestClose", true);
        textDisplay.alpha = 0;
        headerDisplay.alpha = 0;
    }
    public void finishClose()
    {
        CanvasManager.ShowHUD();
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        responseController.close();
        if (readWhenOpen)
            reading = false;
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
        if (source != null)
        {
            source.requestOptionsStart -= promptSelections;
            source.changeHeaderName -= setHeaderName;
            source.startWaitingForInput -= pauseWaitForInputStart;
        }
        source = newSource;
        text = "";
        headerDisplay.text = "";
        newSource.requestOptionsStart += promptSelections;
        newSource.changeHeaderName += setHeaderName;
        newSource.startWaitingForInput += pauseWaitForInputStart;
    }

    public void setHeaderName(string newName)
    {
        headerDisplay.text = newName;
    }

    public void pauseWaitForInputStart()
    {
        
    }

    public void pauseWaitForInputEnd()
    {
        source?.receiveButtonInput();
    }
}
