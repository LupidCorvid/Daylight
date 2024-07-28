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
    public AudioManager.GameArea newArea;

    public Animator spawnedPrompt;

    public Transform promptSpawnLocation;

    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public enum Direction
    {
        LEFT, RIGHT, DOWN, UP
    }
    public Direction direction = Direction.UP;

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
        if(!ChangeScene.changingScene)
            StartCoroutine(LoadNextScene());
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

    IEnumerator LoadNextScene()
    {
        ChangeScene.changingScene = true;
        Crossfade.current.StartFade();
        DialogController.main.closeBox();
        if (newArea != AudioManager.instance.currentArea && newArea != AudioManager.GameArea.CURRENT)
        {
            AudioManager.instance.FadeOutCurrent();
        }
        yield return new WaitForSeconds(1f);
        Player.controller.noFall = true;
        SwordFollow.DisableMovement();
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
        ChangeScene.changingScene = false;
    }

    
}
