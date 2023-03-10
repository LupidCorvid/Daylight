using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class InteractChangeScene : MonoBehaviour, IInteractable
{
    public string scene;
    public string spawn;
    private Animator crossfade;
    public bool noFall = false;

    public Animator spawnedPrompt;

    public Transform promptSpawnLocation;

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interact(GameObject user)
    {
        if(!ChangeScene.changingScene)
            StartCoroutine(LoadNextScene());
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
            spawnedPrompt.SetFloat("InteractType", 3);
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

    IEnumerator LoadNextScene()
    {
        ChangeScene.changingScene = true;
        crossfade.SetTrigger("start");
        DialogController.main.closeBox();
        yield return new WaitForSeconds(1f);
        PlayerMovement.controller.noFall = true;
        CameraController.DisableMovement();
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            GameObject.Destroy(eventSystem.gameObject);
        }
        SceneHelper.LoadScene(scene);
        ChangeScene.clearCollisions?.Invoke();
        ChangeScene.clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        DialogController.closedAnimator = true;
        SpawnManager.spawningAt = spawn;
        Crossfade.changeScene?.Invoke();
    }
}
