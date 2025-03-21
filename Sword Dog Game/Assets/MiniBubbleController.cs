using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MiniBubbleController : MonoBehaviour
{
    public Collider2D mainCamera;
    public DialogNPC speaker;

    public Vector2 setPosition;

    public Vector2 position
    {
        get
        {
            if (speaker != null && !ignoreSpeakerPos)
                return speaker.transform.position;
            else
                return setPosition;
        }
    }

    public bool ignoreSpeakerPos;

    public Collider2D cldr;

    public float minDistance = 0;
    public float maxDistance = 20;

    public Vector2 offset = new Vector2(0, 1.5f);

    public DialogSource dialog;
    public TMPro.TextMeshPro textDisplay;

    bool reading = true, collected = false;

    public string textDialog;

    public List<TextEffect> textEffects = new List<TextEffect>();

    bool closing = false;
    public bool open = false, readout = false;

    public Animator anim;

    public bool enableBarks = false;
    public bool enableVoice = false;
    public float voiceVol;

    // Start is called before the first frame update
    void Start()
    {
        cldr = GetComponent<Collider2D>();
        textDisplay = GetComponentInChildren<TextMeshPro>();

        mainCamera = CameraController.main.cldr;
        if(dialog == null)
            setSource(new DialogSource(textDialog));
        anim = GetComponent<Animator>();

        textDisplay.ForceMeshUpdate();
        textDisplay.OnPreRenderText += applyTextEffects;
        ChangeFont();
    }

    // Update is called once per frame
    void Update()
    {
        if (open && reading && dialog != null)
        {
            if (!collected)
            {
                textDisplay.maxVisibleCharacters = 0;
                textDisplay.text = dialog.collect();
                collected = true;
            }
            dialog.read(DialogSource.ReadMode.TYPEWRITE);
            if (!readout)
            {
                textDisplay.maxVisibleCharacters = dialog.charCount;
            }
        }
        if (collected)
            textDisplay.ForceMeshUpdate();
    }

    private void FixedUpdate()
    {
        Vector2 expectedPlace = position + offset;
        if (mainCamera.OverlapPoint(expectedPlace))
        {
            transform.position += 10 * Time.deltaTime * ((Vector3)expectedPlace - transform.position);
            return;
        }

        Vector2 cameraEdge = mainCamera.ClosestPoint(expectedPlace);
        Vector2 newPosition = new();


        if (cameraEdge.x < mainCamera.transform.position.x)
            newPosition.x = cameraEdge.x + cldr.bounds.extents.x;
        else
            newPosition.x = cameraEdge.x - cldr.bounds.extents.x;

        if (cameraEdge.y < mainCamera.transform.position.y)
            newPosition.y = cameraEdge.y + cldr.bounds.extents.y;
        else
            newPosition.y = cameraEdge.y - cldr.bounds.extents.y;

        if (Vector2.Distance(newPosition, position) > maxDistance)
        {
            if(!closing)
                close();//Start exit and deletion
        }

        transform.position += 10 * Time.deltaTime * ((Vector3)newPosition - transform.position);
    }

    public void setSource(DialogSource newSource)
    {
        if (dialog != null)
        {
            //source.requestOptionsStart -= promptSelections;
            //source.changeHeaderName -= setHeaderName;
            //source.startWaitingForInput -= pauseWaitForInputStart;
            dialog.clear -= OutputCleared;
            dialog.addEffect -= AddEffect;
            dialog.removeEffect -= RemoveEffect;
            dialog.exit -= close;
            dialog.bark -= Bark;
            dialog.speak -= SpeakVoice;
            dialog.pauseSpeak -= PauseSpeak;
            dialog.stopSpeak -= StopSpeak;
            dialog.readout -= ReadOut;
        }
        dialog = newSource;
        if(textDisplay != null)
            textDisplay.text = "";
        textEffects.Clear();
        //newSource.requestOptionsStart += promptSelections;
        //newSource.changeHeaderName += setHeaderName;
        //newSource.startWaitingForInput += pauseWaitForInputStart;
        newSource.clear += OutputCleared;
        newSource.addEffect += AddEffect;
        newSource.removeEffect += RemoveEffect;
        newSource.exit += close;
        newSource.bark += Bark;
        dialog.speak += SpeakVoice;
        dialog.pauseSpeak += PauseSpeak;
        dialog.stopSpeak += StopSpeak;
        dialog.readout += ReadOut;
    }

    public void close()
    {
        reading = false;
        collected = false;
        textDisplay.maxVisibleCharacters = int.MaxValue;

        if (anim != null)
        {
            anim.SetTrigger("Close");
        }
        readout = false;
        closing = true;
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
        for (int i = textEffects.Count - 1; i >= 0; i--)
        {
            if (textEffects[i].type.ToUpperInvariant().Contains(type.ToUpperInvariant()))
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
        for (int i = 1; i < input.Length; i++)
        {
            newInput[i - 1] = input[i];
        }
        TextEffect effect = TextEffect.MakeEffect(newInput, GetLengthNoCommandsRealTime());
        if (effect != null)
            textEffects.Add(effect);
    }

    public int GetLengthNoCommandsRealTime()
    {
        int depth = 0;
        int length = 0;
        for (int i = 0; i < dialog.outString.Length; i++)
        {
            if (dialog.outString[i] == '<')
                depth++;
            if (depth == 0)
                length++;

            if (dialog.outString[i] == '>')
                depth--;
        }
        return length;
    }

    public void OutputCleared()
    {
        textEffects.Clear();
    }

    public void Bark(Vector2 vel, Vector2 acc)
    {
        if (enableBarks)
            speaker?.barkEffect(vel, acc);
    }

    public void SpeakVoice()
    {
        //Maybe make it quieter when it speaks this way?
        if(enableVoice)
            speaker?.speakVoice(voiceVol);
    }

    public void PauseSpeak()
    {
        if (enableVoice)
            speaker?.pauseSpeak();
        
    }

    public void ReadOut()
    {
        readout = true;
        textDisplay.maxVisibleCharacters = int.MaxValue;
    }

    public void StopSpeak()
    {
        speaker?.stopSpeak();
    }

    public void ChangeFont()
    {
        textDisplay.font = FontManager.main.currFont;
    }

    public void SetBackground(bool visible)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = visible;
    }
}
