using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractablesTracker : MonoBehaviour
{
    public List<IInteractable> interactables = new List<IInteractable>();
    public IInteractable nearest;

    //On enter start animation for appearing, on exit start animation that ends in deletion
    public GameObject interactPrompt;

    //public static KeyCode interactKey = KeyCode.T;
    public static bool alreadyInteracting = false;
    public Entity attachedEntity;

    private float delay = 0f;
    private float maxDelay = 0.5f;

    private void Awake()
    {

    }

    private void FixedUpdate()
    {
        if (delay < maxDelay)
            delay += Time.fixedDeltaTime;

        if ((CutsceneController.cutsceneStopInteractions || MenuManager.inMenu) && nearest != null)
        {
            nearest.hidePrompt();
            nearest = null;
        }
        
        if (delay >= maxDelay && !alreadyInteracting && !ChangeScene.changingScene && !CutsceneController.cutsceneStopInteractions && !MenuManager.inMenu)
        {
            IInteractable newNearest = getNearest();
            if (nearest != newNearest)
            {
                if (nearest != null)
                {
                    nearest.hidePrompt();
                    nearest.inRange = false;
                }
                nearest = newNearest;
                if (nearest != null)
                {
                    nearest.showPrompt(interactPrompt);
                    nearest.inRange = true;
                }
            }
        }
    }

    private void Update()
    {
        if (delay >= maxDelay && InputReader.inputs.actions["Interact"].WasPressedThisFrame())
        {
            //nearest?.interact(transform.parent.gameObject);
            nearest?.interact(attachedEntity);
        }
    }

    private void Start()
    {
        ChangeScene.clearInteractables += ClearAll;
    }

    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
            interactables.Add(collision.GetComponent<IInteractable>());
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
            interactables.Remove(collision.GetComponent<IInteractable>());
    }

    public IInteractable getNearest()
    {
        (IInteractable interactable, float distance) closest = (null, 99999);
        bool needsClean = false;

        foreach (IInteractable interactable in interactables)
        {
            if (interactable == null || interactable.gameObject == null || transform == null)
            {
                needsClean = true;
                continue;
            }
            float distance = Vector2.Distance(interactable.gameObject.transform.position, transform.position);
            if (distance < closest.distance)
            {
                closest = (interactable, distance);
            }
        }

        if (needsClean)
            clean();

        return closest.interactable;
    }

    private void ClearAll()
    {
        interactables.Clear();
        nearest = null;
        delay = 0f;
    }

    public void ClearNearest()
    {
        nearest = null;
    }

    /// <summary>
    /// Removes all null variables from the list
    /// </summary>
    public void clean()
    {
        for(int i = interactables.Count - 1; i >= 0; i--)
        {
            if (interactables[i] == null)
                interactables.RemoveAt(i);
        }
    }
}
