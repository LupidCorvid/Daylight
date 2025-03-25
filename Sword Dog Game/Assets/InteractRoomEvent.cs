using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractRoomEvent : MonoBehaviour, IInteractable
{
    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public int iconType = 2;

    public Transform promptSpawnLocation;

    public Animator spawnedPrompt;

    public string eventName;

    public Action interactedWith; //Actions: An array of function pointers that will call the functions when the action is invoked

    public virtual void interact(Entity user)
    {
        RoomManager.currentRoom.callRoomEvent(eventName);
        interactedWith?.Invoke(); //Call this if it's not null
        hidePrompt();
    }

    public virtual void OnDestroy()
    {
        hidePrompt();
    }

    public virtual void hidePrompt()
    {
        if (spawnedPrompt != null)
        {
            spawnedPrompt.SetTrigger("Close");
            spawnedPrompt.SetFloat("Speed", 1);
        }
    }

    public virtual void showPrompt(GameObject prompt)
    {
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
            spawnedPrompt.SetFloat("InteractType", iconType);
        }
        else
        {
            spawnedPrompt.SetTrigger("Reopen");
            spawnedPrompt.SetFloat("Speed", -1);
        }
    }
}
