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

    public static bool dialogOpen
    {
        get { return main.reading; }
    }

    public DialogResponseController responseController;

    public Animator anim;

    public bool readWhenOpen = false;

    public List<TextEffect> textEffects = new List<TextEffect>();

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

    public static bool closedAnimator = true;

    void Awake()
    {
        main = this;
        textDisplay.ForceMeshUpdate();

        textDisplay.OnPreRenderText += applyTextEffects;

        ChangeScene.changeScene += closeBox;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Return))
            pauseWaitForInputEnd();
        if (reading)
        {
            textDisplay.text = source.read();
            textDisplay.ForceMeshUpdate();
            
        }
    }

    public void finishOpen()
    {
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
        closedAnimator = false;
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
        closedAnimator = true;
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
            source.clear -= OutputCleared;
            source.addEffect -= AddEffect;
            source.removeEffect -= RemoveEffect;
        }
        source = newSource;
        text = "";
        headerDisplay.text = "";
        textEffects.Clear();
        newSource.requestOptionsStart += promptSelections;
        newSource.changeHeaderName += setHeaderName;
        newSource.startWaitingForInput += pauseWaitForInputStart;
        newSource.clear += OutputCleared;
        newSource.addEffect += AddEffect;
        newSource.removeEffect += RemoveEffect;

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

    public void applyTextEffects(TMP_TextInfo info)
    {
        for (int i = 0; i < textEffects.Count; i++)
        {
            textEffects[i].ApplyEffectToMesh(info);
        }
    }

    public void RemoveEffect(string type)
    {
        if (type[0] == ' ')
            type = type.Substring(1);
        for(int i = textEffects.Count - 1; i >= 0; i--)
        {
            if(textEffects[i].type.ToUpperInvariant().Contains(type.ToUpperInvariant()))
            {
                textEffects[i].end = GetLengthNoCommands();
                return;
            }
        }
        Debug.LogWarning("There was no effect of type " + type + " to end!");
    }

    public void AddEffect(string[] input)
    {
        string[] newInput = new string[input.Length - 1];
        for(int i = 1; i < input.Length; i++)
        {
            newInput[i - 1] = input[i];
        }
        TextEffect effect = TextEffect.MakeEffect(newInput, GetLengthNoCommands());
        if(effect != null)
            textEffects.Add(effect);
    }

    public void OutputCleared()
    {
        textEffects.Clear();
    }

    public int GetLengthNoCommands()
    {
        int depth = 0;
        int length = 0;
        for(int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<')
                depth++;
            if (depth == 0)
                length++;

            if (text[i] == '>')
                depth--;
        }
        return length;
    }

}
