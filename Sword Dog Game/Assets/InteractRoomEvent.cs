using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void interact(GameObject user)
    {
        RoomManager.currentRoom.callRoomEvent(eventName);
        hidePrompt();
    }

    public void OnDestroy()
    {
        hidePrompt();
    }

    public void hidePrompt()
    {
        if (spawnedPrompt != null)
            spawnedPrompt.SetTrigger("Close");
    }

    public void showPrompt(GameObject prompt)
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
        }
    }
}