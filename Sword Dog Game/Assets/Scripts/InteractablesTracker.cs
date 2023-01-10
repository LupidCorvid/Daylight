using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesTracker : MonoBehaviour
{
    public List<IInteractable> interactables = new List<IInteractable>();
    public IInteractable nearest;

    //On enter start animation for appearing, on exit start animation that ends in deletion
    public GameObject interactPrompt;
    public static KeyCode interactKey = KeyCode.T;
    public static bool alreadyInteracting = false;

    private void FixedUpdate()
    {
        if (!alreadyInteracting && !ChangeScene.changingScene && !CutsceneController.cutsceneStopInteractions)
        {
            IInteractable newNearest = getNearest();
            if (nearest != newNearest)
            {
                if (nearest != null)
                {
                    nearest.hidePrompt(interactPrompt);
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
        if (Input.GetKeyDown(interactKey))
        {
            nearest?.interact(transform.parent.gameObject);
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
