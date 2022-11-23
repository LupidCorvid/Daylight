using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesTracker : MonoBehaviour
{
    public List<IInteractable> interactables = new List<IInteractable>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            getNearest()?.interact();
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
        (IInteractable interactable, float distance) nearest = (null, 99999);

        foreach(IInteractable interactable in interactables)
        {
            float distance = Vector2.Distance(interactable.gameObject.transform.position, transform.position);
            if (distance < nearest.distance)
            {
                nearest = (interactable, distance);
            }
        }
        return nearest.interactable;
    }

    private void ClearAll()
    {
        interactables.Clear();
    }

}
