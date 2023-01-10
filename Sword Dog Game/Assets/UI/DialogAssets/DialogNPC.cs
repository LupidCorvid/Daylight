using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogNPC : MonoBehaviour, IInteractable
{
    public GameObject barkFXPrefab;

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

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public GameObject interactor;

    public Action closedDialog;

    public void Awake()
    {
        if (dialog.Count > 0)
            dialogSource = new DialogSource(dialog[numInteractions % dialog.Count]);
        else
            dialogSource = new DialogSource("[exit]");
        dialogSource.bark += barkEffect;
        //dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
    }
    public virtual void interact(GameObject user)
    {
        if (!alreadyTalking && !stoppedTalkingThisFrame && !DialogController.dialogOpen && DialogController.closedAnimator)
        {
            interactor = user;
            if (loopInteractions)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            else if (numInteractions < dialog.Count)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            openDialog();
            //hidePrompt(null);
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

        if(interactor?.GetComponentInChildren<InteractablesTracker>()?.nearest != null)
            interactor.GetComponentInChildren<InteractablesTracker>().nearest.hidePrompt(null);
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
            //dialogSource.barkDefault -= barkEffect;
            dialogSource.exit -= exitDialog;
        }
        dialogSource = newSource;
        dialogSource.callEvent += eventCalled;
        dialogSource.bark += barkEffect;
        //dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
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
        Invoke("tryShowPrompt", 1.5f);
        interactor = null;
        closedDialog?.Invoke();
    }

    private void tryShowPrompt()
    {
        if (inRange)
            showPrompt(promptPrefab);
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

    public void hidePrompt(GameObject prompt)
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

}
