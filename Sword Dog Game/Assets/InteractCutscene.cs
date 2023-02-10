using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCutscene : MonoBehaviour, IInteractable
{

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public Transform promptSpawnLocation;

    public Animator spawnedPrompt;

    public int iconType = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void interact(GameObject user)
    {
        CutsceneController.PlayCutscene("MountainView");
            
    }

    public void hidePrompt(GameObject prompt)
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
