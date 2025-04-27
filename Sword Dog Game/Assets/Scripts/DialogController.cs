using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DialogController : MonoBehaviour
{
    public static DialogController main;
    public static DialogNPC speaker;

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

    public bool endedWaitThisFrame = false;

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
    public bool openedThisFrame = false;
    public bool gotResponseThisFrame = false;
    public bool skippedTextThisFrame = false;

    public bool inDialog = false;

    public Animator DotAnimator;

    public bool collected = false;

    public bool pausePlayerMovement = false;

    public TMPro.TextMeshProUGUI contText;

    void Awake()
    {
        main ??= this;
        textDisplay.ForceMeshUpdate();

        textDisplay.OnPreRenderText += applyTextEffects;

        ChangeScene.changeScene += closeBox;
    }

    public void ChangeFont()
    {
        textDisplay.font = FontManager.main.currFont;
        headerDisplay.font = FontManager.main.currFont;
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (InputReader.inputs == null) return; // temp
        //if (main == null || main != this)
        //    main = this;

        //Copy of No Laughing Matter vers.
        if ((InputReader.inputs.actions["Interact"].WasPressedThisFrame()) && source != null)
        {
            if (!(source.waiting || source.waitingForButtonInput) && !openedThisFrame && reading && !gotResponseThisFrame && source.skippableText)
            {
                source.skippingText = true;
                skippedTextThisFrame = true;
            }
            pauseWaitForInputEnd();
        }
        if (reading)
        {
            if (!collected)
            {
                //textDisplay.maxVisibleCharacters = 0;
                textDisplay.text = source.collect();
                collected = true;
            }
            source.read(DialogSource.ReadMode.TYPEWRITE);
            textDisplay.maxVisibleCharacters = source.charCount; //might have issues with TMPPro stuff? 
        }
        if (collected)
            textDisplay.ForceMeshUpdate();

    }

    private void LateUpdate()
    {
        openedThisFrame = false;
        gotResponseThisFrame = false;
        endedWaitThisFrame = false;
        skippedTextThisFrame = false;
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
        DotAnimator.ResetTrigger("Close");
        inDialog = true;
        ChangeFont();
    }
    public void closeBox()
    {
        if(anim != null)
            anim?.SetBool("requestClose", true);
        closedAnimator = false;
        if(textDisplay != null)
            textDisplay.alpha = 0;
        if(headerDisplay != null)
           headerDisplay.alpha = 0;
        DotAnimator?.SetTrigger("Close");
        inDialog = false;
        pausePlayerMovement = false;
        
    }


    public void finishClose()
    {
        if (!CutsceneController.cutsceneHideUI && !PauseScreen.quit && SceneManager.GetActiveScene().name != "Main menu")
            CanvasManager.ShowHUD();
        if (panel != null)
        {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        responseController?.close();
        if (readWhenOpen)
            reading = false;
        closedAnimator = true;
        InteractablesTracker.alreadyInteracting = false;
        CanvasManager.main.DisplayQueuedNotif();
    }

    public void forceClose()
    {

        anim.Play("Idle");
        textDisplay.alpha = 0;
        headerDisplay.alpha = 0;


        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        responseController.close();
        if (readWhenOpen)
            reading = false;
        closedAnimator = true;
        InteractablesTracker.alreadyInteracting = false;
    }

    public void promptSelections(params string[] options)
    {
        responseController.setResponses(options);
        
    }
    public void receiveResponse(int response)
    {
        source.receiveResponse(response);
        responseController.close();
        gotResponseThisFrame = true;
        collected = false;
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
        source.position = 0;
        collected = false;
        textEffects.Clear();
        newSource.requestOptionsStart += promptSelections;
        newSource.changeHeaderName += setHeaderName;
        newSource.startWaitingForInput += pauseWaitForInputStart;
        newSource.clear += OutputCleared;
        newSource.addEffect += AddEffect;
        newSource.removeEffect += RemoveEffect;

    }

    public void setSpeaker(DialogNPC speaker)
    {
        DialogController.speaker = speaker;
    }

    public void setHeaderName(string newName)
    {
        headerDisplay.text = newName;
    }

    public void pauseWaitForInputStart()
    {
        contText.text = replaceBinding("Interact");
        DotAnimator.SetTrigger("Open");
    }

    public string renameInput(string actionNeeded, string inputName)
    {
        Debug.Log(inputName);
        inputName = inputName.Replace(actionNeeded + ":", string.Empty);
        inputName = inputName.Replace("<Keyboard>/", "Keyboard_");
        inputName = inputName.Replace("/Keyboard/", "Keyboard_");
        inputName = inputName.Replace("[Keyboard]", "");
        inputName = inputName.Replace("<Mouse>/", "Mouse_");
        inputName = inputName.Replace("[Mouse]", "");
        inputName = inputName.Replace("<Gamepad>/", "Xbox_");
        inputName = inputName.Replace("[Gamepad]", "");
        return inputName;
    }

    public string replaceBinding(string actionNeeded)
    {
        InputBinding binding = InputReader.inputs.actions[actionNeeded].bindings[(int)InputReader.deviceType];
        TMP_SpriteAsset spriteAsset = InputReader.spriteAssets[(int)InputReader.deviceType];
        string inputName = renameInput(actionNeeded, binding.ToString());
        return $"<sprite=\"{spriteAsset.name}\" name=\"{inputName}\">";
    }

    public void pauseWaitForInputEnd()
    {
        if (source?.waitingForButtonInput == true)
        {
            DotAnimator.SetTrigger("Close");
            endedWaitThisFrame = true;
        }
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
                textEffects[i].end = GetLengthNoCommandsRealTime();
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
        TextEffect effect = TextEffect.MakeEffect(newInput, GetLengthNoCommandsRealTime());
        if(effect != null)
            textEffects.Add(effect);
    }

    public void OutputCleared()
    {
        textEffects.Clear();
        //source.position--;
        
        collected = false;
        
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

    /// <summary>
    /// Reads the next part of the string that hasnt been "pushed" yet instead of what is already out
    /// </summary>
    /// <returns></returns>
    public int GetLengthNoCommandsRealTime()
    {
        int depth = 0;
        int length = 0;
        for (int i = 0; i < source.outString.Length; i++)
        {
            if (source.outString[i] == '<')
                depth++;
            if (depth == 0)
                length++;

            if (source.outString[i] == '>')
                depth--;
        }
        return length;
    }

}
