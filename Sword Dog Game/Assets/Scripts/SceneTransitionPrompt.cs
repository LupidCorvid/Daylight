using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneTransitionPrompt : MonoBehaviour, IInteractable
{
    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;
    public enum Direction
    {
        LEFT, RIGHT, DOWN, UP
    }
    public Direction direction;
    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void interact(Entity user)
    {
        // do nothing -- this is not an interactable transition
        // just extending off of interactable interface because lazy
    }

    public void showPrompt(GameObject prompt)
    {
        if (spawnedPrompt == null)
        {
            GameObject addedPrompt;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            switch (direction)
            {
                case Direction.DOWN:
                    rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Direction.LEFT:
                    rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.RIGHT:
                    rotation = Quaternion.Euler(0, 0, -90);
                    break;
            }
            if (promptSpawnLocation == null)
                addedPrompt = Instantiate(prompt, transform.position + (1 * Vector3.up), rotation);
            else
            {
                addedPrompt = Instantiate(prompt, promptSpawnLocation.position, rotation);
                addedPrompt.transform.localScale = promptSpawnLocation.localScale;
            }
            spawnedPrompt = addedPrompt.GetComponent<Animator>();
            spawnedPrompt.SetFloat("InteractType", 3);
        }
        else
        {
            spawnedPrompt.SetTrigger("Reopen");
            spawnedPrompt.SetFloat("Speed", -1);
        }
    }

    public void hidePrompt()
    {
        if (spawnedPrompt != null)
        {
            spawnedPrompt.SetTrigger("Close");
            spawnedPrompt.SetFloat("Speed", 1);
        }
    }
}
