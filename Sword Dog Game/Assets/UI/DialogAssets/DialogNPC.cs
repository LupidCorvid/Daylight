using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogNPC : MonoBehaviour, IInteractable
{
    public GameObject barkFXPrefab;
    public GameObject miniBubblePrefab;
    public GameObject emotePrefab;


    public Vector2 miniBubbleOffset = new Vector2(0, 2);

    public DialogSource dialogSource;

    //public string dialog;

    public List<string> dialog;

    public int numInteractions = 0;

    public bool loopInteractions;

    public bool alreadyTalking;

    bool stoppedTalkingThisFrame = false;

    public GameObject barkSpawnLocation;

    public GameObject promptPrefab;
    public Animator spawnedPrompt;

    public Transform promptSpawnLocation;

    public Transform emoteSpawnLocation;

    public SoundClip speechLoop;
    public SoundPlayer soundPlayer;
    public bool speaking, pausedSpeak;

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public Entity interactor;

    public Action closedDialog;

    public void Awake()
    {
        if (dialog.Count > 0)
            dialogSource = new DialogSource(dialog[numInteractions % dialog.Count]);
        else
            dialogSource = new DialogSource("[exit]");
        dialogSource.bark += barkEffect;
        dialogSource.ps += playSound;
        dialogSource.es += endSound;
        //dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
        dialogSource.speak += speakVoice;
        dialogSource.pauseSpeak += pauseSpeak;
        dialogSource.stopSpeak += stopSpeak;
    }
    public virtual void interact(Entity user)
    {
        if (!alreadyTalking && !stoppedTalkingThisFrame && !DialogController.dialogOpen && DialogController.closedAnimator)
        {
            interactor = user;
            if (loopInteractions)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            else if (numInteractions < dialog.Count)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            openDialog();
        }
    }

    public virtual void openDialog()
    {
        dialogSource.position = 0;
        dialogSource.resetDialog();
        DialogController.main.openedThisFrame = true;
        DialogController.main.setSource(dialogSource);
        DialogController.main.openBox();
        DialogController.main.readWhenOpen = true;
        numInteractions++;
        alreadyTalking = true;
        InteractablesTracker.alreadyInteracting = true;

        interactor?.GetComponentInChildren<InteractablesTracker>()?.nearest?.hidePrompt();
    }

    private void LateUpdate()
    {
        stoppedTalkingThisFrame = false;
    }

    public void setNewSource(DialogSource newSource)
    {
        if(dialogSource != null)
        {
            dialogSource.callEvent -= eventCalled;
            dialogSource.bark -= barkEffect;
            dialogSource.ps -= playSound;
            dialogSource.es -= endSound;
            //dialogSource.barkDefault -= barkEffect;
            dialogSource.exit -= exitDialog;
            dialogSource.speak -= speakVoice;
            dialogSource.pauseSpeak -= pauseSpeak;
            dialogSource.stopSpeak -= stopSpeak;
            dialogSource.emote -= spawnEmote;
            dialogSource.reemote -= spawnReEmote;
        }
        dialogSource = newSource;
        dialogSource.callEvent += eventCalled;
        dialogSource.bark += barkEffect;
        dialogSource.ps += playSound;
        dialogSource.es += endSound;
        //dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
        dialogSource.speak += speakVoice;
        dialogSource.pauseSpeak += pauseSpeak;
        dialogSource.stopSpeak += stopSpeak;
        dialogSource.emote += spawnEmote;
        dialogSource.reemote += spawnReEmote;
        
    }

    public virtual void eventCalled(params string[] input)
    {

    }

    public virtual void exitDialog()
    {
        DialogController.main.closeBox();
        DialogController.main.reading = false;
        alreadyTalking = false;
        stoppedTalkingThisFrame = true;
        interactor = null;
        closedDialog?.Invoke();
    }

    public void barkEffect()
    {
        GameObject addedObject = Instantiate(barkFXPrefab, barkSpawnLocation?.transform?.position ?? transform.position, barkSpawnLocation?.transform?.rotation ?? transform.rotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity.y = -1;
        addedParticle.acceleration.y = 3;
        addedParticle.startTime = Time.time;
    }
    public void barkEffect(Vector2 velocity, Vector2 acceleration)
    {
        Vector2 spawnLocation = transform.position;
        Quaternion spawnRotation = transform.rotation;

        if(barkSpawnLocation != null)
        {
            spawnLocation = barkSpawnLocation.transform.position;
            spawnRotation = barkSpawnLocation.transform.rotation;
        }

        GameObject addedObject = Instantiate(barkFXPrefab, spawnLocation, spawnRotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity = velocity;
        addedParticle.acceleration = acceleration;
        addedParticle.startTime = Time.time;
    }

    public void playSound(string sound, float volume = 1, bool loop = false)
    {
        soundPlayer ??= GetComponentInChildren<SoundPlayer>();
        if (soundPlayer == null)
            Debug.LogError("No sound player attached to this NPC");
        soundPlayer?.PlaySound(sound, volume, loop);
    }

    public void endSound(string sound = null)
    {
        soundPlayer ??= GetComponentInChildren<SoundPlayer>();
        if (soundPlayer == null)
            Debug.LogError("No sound player attached to this NPC");
        soundPlayer?.EndSound(sound);
    }

    public void hidePrompt()
    {
        if (spawnedPrompt != null)
            spawnedPrompt.SetTrigger("Close");
    }

    public void showPrompt(GameObject prompt)
    {
        if (alreadyTalking)
            return;
        if (spawnedPrompt == null)
        {
            GameObject addedPrompt;
            if (promptSpawnLocation == null)
                addedPrompt = Instantiate(prompt, transform.position + (1 * Vector3.up), transform.rotation);
            else
            {
                addedPrompt = Instantiate(prompt, promptSpawnLocation.position, promptSpawnLocation.rotation);
                addedPrompt.transform.localScale = promptSpawnLocation.localScale;
            }
            spawnedPrompt = addedPrompt.GetComponent<Animator>();
            spawnedPrompt.SetFloat("InteractType", 1);
        }
        else
        {
            spawnedPrompt.SetTrigger("Reopen");
        }
    }

    public void SpeakMiniBubble()
    {
        GameObject addedObj = Instantiate(miniBubblePrefab, transform.position + (Vector3)miniBubbleOffset, Quaternion.identity);
        MiniBubbleController bubble = addedObj.GetComponent<MiniBubbleController>();
        bubble.speaker = this;
        bubble.offset = miniBubbleOffset;
        bubble.setSource(new DialogSource("[ss, .05][IA,<size=125%><align=center><margin-right=0.5em>]Interested in buying anything?[w, 1] [exit]"));

    }

    public void speakVoice()
    {
        if (!speaking)
        {
            soundPlayer?.PlaySound(speechLoop, 0.5f, true);
            speaking = true;
        }
        else if (pausedSpeak)
        {
            soundPlayer?.UnPauseSound(speechLoop);
            pausedSpeak = false;
        }
    }

    public void pauseSpeak()
    {
        if (speaking && !pausedSpeak)
        {
            soundPlayer?.PauseSound(speechLoop);
            pausedSpeak = true;
        }
    }

    public void stopSpeak()
    {
        if (speaking)
        {
            soundPlayer?.EndSound(speechLoop);
            pausedSpeak = false;
            speaking = false;
        }
    }

    public void spawnEmote(int type, float lifeTime)
    {
        GameObject addedObj = Instantiate(emotePrefab, emoteSpawnLocation.transform.position, emoteSpawnLocation.transform.rotation, emoteSpawnLocation.transform);
        GeneralEmote spawnedEmote = addedObj.GetComponent<GeneralEmote>();
        spawnedEmote.type = type;
        spawnedEmote.lifeTime = lifeTime;
    }


    public void spawnReEmote(int type, float lifeTime)
    {
        GameObject addedObj = Instantiate(emotePrefab, interactor.emotePosition.position, interactor.emotePosition.rotation, interactor.emotePosition);
        GeneralEmote spawnedEmote = addedObj.GetComponent<GeneralEmote>();
        spawnedEmote.type = type;
        spawnedEmote.lifeTime = lifeTime;
    }
}
