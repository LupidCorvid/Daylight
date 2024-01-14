using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public bool inRange { get; set; }
    public Transform promptSpawnLocation;
    public Animator spawnedPrompt;

    public CutsceneController SaveCutscene;
    public CutsceneController SaveExitCutscene;

    public bool waitingForCutsceneFinish = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveCutscene.cutsceneNumber > 0 && SaveCutscene.playingThisCutscene)
            waitingForCutsceneFinish = true;

        if(waitingForCutsceneFinish && !SaveCutscene.playingThisCutscene)
        {
            //Open save prompt
            SaveGamePrompt.main.openDialog(this);
            waitingForCutsceneFinish = false;

        }
    }

    public void FinishedSavePrompt()
    {
        SaveExitCutscene.StartCutscene();
    }

    public void interact(Entity user)
    {
        hidePrompt();
        if(!SaveCutscene.playingThisCutscene)
            SaveCutscene.StartCutscene();
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
