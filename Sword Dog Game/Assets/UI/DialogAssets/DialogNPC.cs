using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNPC : MonoBehaviour, IInteractable
{
    public GameObject barkFXPrefab;

    public DialogSource dialogSource;

    public string dialog;

    public void Awake()
    {
        dialogSource = new DialogSource(dialog);
        dialogSource.bark += barkEffect;
        dialogSource.barkDefault += barkEffect;
        dialogSource.exit += exitDialog;
    }
    public void interact()
    {
        dialogSource.position = 0;
        DialogController.main.source = dialogSource;
        DialogController.main.openBox();
        DialogController.main.reading = true;
    }

    public void exitDialog()
    {
        DialogController.main.closeBox();
        DialogController.main.reading = false;
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
