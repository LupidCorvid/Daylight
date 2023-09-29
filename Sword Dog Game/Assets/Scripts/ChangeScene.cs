using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ChangeScene : MonoBehaviour, IInteractable
{
    public string scene;
    public string spawn;
    private Animator crossfade;
    public static bool changingScene = false;
    public static Action clearCollisions, clearInteractables;
    public Animator spawnedPrompt;
    public Transform promptSpawnLocation;
    public enum Direction {
        LEFT, RIGHT, DOWN, UP
    }
    public Direction direction;
    private bool _inRange = false;
    public bool inRange
    {
        get { return _inRange; }
        set { _inRange = value; }
    }

    public static Action changeScene;

    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!changingScene && collision.gameObject.CompareTag("Player") && !PlayerHealth.dead && !PlayerMovement.controller.resetting && !GameSaver.loading)
        {
            StartCoroutine(LoadNextScene());
        }
    }
    IEnumerator LoadNextScene()
    {
        changingScene = true;
        crossfade.SetTrigger("start");
        DialogController.main.closeBox();
        yield return new WaitForSeconds(1f);
        DisableMenuMusic();
        PlayerMovement.controller.noFall = true;
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        GameObject.Destroy(eventSystem?.gameObject);
        SceneHelper.LoadScene(scene);
        clearCollisions?.Invoke();
        clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        DialogController.closedAnimator = true;
        SpawnManager.spawningAt = spawn;
        Crossfade.changeScene?.Invoke();
        changingScene = false;
    }

    public static void ChangeSceneMinimal(string scene)
    {
        changingScene = true;
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        GameObject.Destroy(eventSystem?.gameObject);
        DisableMenuMusic();
        PlayerMovement.controller.noFall = true;
        SceneHelper.LoadScene(scene);
        clearCollisions?.Invoke();
        clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        //DialogController.closedAnimator = true;
        changingScene = false;
    }

    public static void LoadScene(string scene, string spawn = "", bool showHUD = true)
    {
        GameSaver.main.StartCoroutine(LoadSceneEnum(scene, spawn, showHUD));
        
    }

    static IEnumerator LoadSceneEnum(string scene, string spawn, bool showHUD = true)
    {
        changingScene = true;
        Crossfade.current.StartFade();
        DialogController.main.closeBox();
        yield return new WaitForSeconds(1f);
        DisableMenuMusic();
        if(PlayerMovement.controller != null)
            PlayerMovement.controller.noFall = true;
        SwordFollow.DisableMovement();
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            GameObject.Destroy(eventSystem.gameObject);
        }
        SceneHelper.LoadScene(scene);
        clearCollisions?.Invoke();
        clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        DialogController.closedAnimator = true;
        SpawnManager.spawningAt = spawn;
        //Crossfade.changeScene?.Invoke();
        Crossfade.current.StopFade();
        if (showHUD)
            CanvasManager.InstantShowHUD();
        changingScene = false;
    }

    public static void DisableMenuMusic()
    {
        if (MainMenuManager.inMainMenu)
        {
            AudioManager.instance.Stop();
            MainMenuManager.inMainMenu = false;
        }
    }

    public void showPrompt(GameObject prompt)
    {
        if (spawnedPrompt == null)
        {
            GameObject addedPrompt;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            switch (direction) {
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
        }
    }

    public void hidePrompt()
    {
        spawnedPrompt?.SetTrigger("Close");
    }
}