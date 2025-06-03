using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickupBehavior : InteractRoomEvent
{
    public GameObject tutorialPrompt;
    private MiniBubbleController bubble = null;
    public GameObject miniBubblePrefab;
    public Vector2 miniBubbleOffset = new(0, 2);
    public Animator swordAnim;
    private bool interacted = false;
    public AK.Wwise.Event NextPhaseEvent;

    public void spawnPrompt()
    {
        // Prompt is always there until sword is picked up
        GameObject addedPrompt;
        if (promptSpawnLocation == null)
            addedPrompt = Instantiate(tutorialPrompt, transform.position + (1 * Vector3.up), transform.rotation);
        else
        {
            addedPrompt = Instantiate(tutorialPrompt, promptSpawnLocation.position, promptSpawnLocation.rotation);
            addedPrompt.transform.localScale = promptSpawnLocation.localScale;
        }
        spawnedPrompt = addedPrompt.GetComponent<Animator>();
    }

    public override void interact(Entity user)
    {
        RoomManager.currentRoom.callRoomEvent(eventName);
        actuallyHidePrompt();
        NextPhaseEvent?.Post(AudioManager.WwiseGlobal);
        interacted = true;
    }

    public void actuallyHidePrompt()
    {
        if (spawnedPrompt != null)
        {
            spawnedPrompt.SetTrigger("Close");
            if (spawnedPrompt.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5)
            {
                spawnedPrompt.SetFloat("Speed", -2);
            }
        }
        if (bubble != null)
        {
            bubble.close();
        }
    }

    public override void hidePrompt()
    {
        if (interacted) return;
        if (spawnedPrompt != null)
        {
            spawnedPrompt.SetFloat("Speed", -1);
            if (spawnedPrompt.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                spawnedPrompt.Play(spawnedPrompt.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 1);
            }
        }
        if (bubble != null)
        {
            bubble.close();
            bubble = null;
        }
        if (swordAnim != null)
        {
            if (swordAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                swordAnim.Play(swordAnim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 1);
            }
            swordAnim.SetFloat("Speed", -1);
        }
    }

    public override void showPrompt(GameObject prompt)
    {
        spawnedPrompt?.SetTrigger("OpenBubble");
        spawnedPrompt?.SetFloat("Speed", 1);

        if (bubble == null)
        {
            Vector3 position = transform.position + (Vector3)miniBubbleOffset;
            GameObject addedObj = Instantiate(miniBubblePrefab, position, Quaternion.identity);
            bubble = addedObj.GetComponent<MiniBubbleController>();
            bubble.SetBackground(false);
            bubble.offset = miniBubbleOffset;
            bubble.setPosition = position;
            // TODO figure out how to insert an input prompt icon here
            bubble.setSource(new DialogSource("[ss, 0.035] [IA,<size=120%><align=center><margin-right=0.1em><voffset=0.0em>] [btn, Interact] to interact [wi] [exit]"));
        }

        if (swordAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0)
        {
            swordAnim.Play(swordAnim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
        }
        swordAnim.SetFloat("Speed", 1);
    }
}
