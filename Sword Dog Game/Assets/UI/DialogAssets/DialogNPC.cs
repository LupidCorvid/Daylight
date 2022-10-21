using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Awake()
    {
        dialogSource = new DialogSource(dialog[numInteractions % dialog.Count]);
        dialogSource.bark += barkEffect;
        dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
    }
    public void interact()
    {
        if (!alreadyTalking && !stoppedTalkingThisFrame)
        {
            if (loopInteractions)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            else if (numInteractions < dialog.Count)
                setNewSource(new DialogSource(dialog[numInteractions % dialog.Count]));
            dialogSource.position = 0;
            dialogSource.resetDialog();
            //DialogController.main.source = dialogSource;
            DialogController.main.setSource(dialogSource);
            DialogController.main.openBox();
            DialogController.main.readWhenOpen = true;
            numInteractions++;
            alreadyTalking = true;
        }
    }

    private void LateUpdate()
    {
        stoppedTalkingThisFrame = false;
    }

    public void setNewSource(DialogSource newSource)
    {
        if(dialogSource != null)
        {
            dialogSource.bark -= barkEffect;
            dialogSource.barkDefault -= barkEffect;
            dialogSource.exit -= exitDialog;
        }
        dialogSource = newSource;
        dialogSource.bark += barkEffect;
        dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
    }

    public void exitDialog()
    {
        DialogController.main.closeBox();
        DialogController.main.reading = false;
        alreadyTalking = false;
        stoppedTalkingThisFrame = true;
    }
    public void barkEffect()
    {
        GameObject addedObject = Instantiate(barkFXPrefab, transform.position, transform.rotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity.y = -1;
        addedParticle.acceleration.y = 3;
        addedParticle.startTime = Time.time;
    }
    public void barkEffect(Vector2 velocity, Vector2 acceleration)
    {
        GameObject addedObject = Instantiate(barkFXPrefab, transform.position, transform.rotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity = velocity;
        addedParticle.acceleration = acceleration;
        addedParticle.startTime = Time.time;
    }
}
