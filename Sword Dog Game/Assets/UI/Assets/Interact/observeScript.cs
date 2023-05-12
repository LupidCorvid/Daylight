using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class observeScript : MonoBehaviour, IInteractable
{

    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;
    public GameObject prompt;
    bool isInteracting;
    private MiniBubbleController bubble;
    public GameObject miniBubblePrefab;

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public string content;

    // Start is called before the first frame update
    void Start()
    {
        isInteracting = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    public void interact(Entity user)
    {
        if (bubble != null)
            return;
        //Make text bubble appear
        isInteracting = true;
        hidePrompt();
        print("A mini bubble should open");
        SpeakMiniBubble();
    }

    //When the user is done looking at the sign OR user walks away
    public void closeInteract()
    {
        isInteracting = false;
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
            spawnedPrompt.SetFloat("InteractType", 2);
        }
        else
        {
            spawnedPrompt.SetTrigger("Reopen");
        }
    }

    public void hidePrompt()
    {
        if (spawnedPrompt != null)
            spawnedPrompt.SetTrigger("Close");
    }

    public void SpeakMiniBubble()
    {
        GameObject addedObj = Instantiate(miniBubblePrefab, promptSpawnLocation.position, Quaternion.identity, TempObjectsHolder.asTransform);
        bubble = addedObj.GetComponent<MiniBubbleController>();
        bubble.setPosition = promptSpawnLocation.position;
        bubble.setSource(new DialogSource(content));
        //bubble.open = true;

    }
}
