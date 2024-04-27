using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ChangeScene : MonoBehaviour
{
    public string scene;
    public string spawn;
    public static bool changingScene = false;
    public static Action clearCollisions, clearInteractables;
    public static Action changeScene;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
        if (!changingScene && collision.gameObject.CompareTag("Player") && !PlayerHealth.dead && Player.controller != null && !Player.controller.resetting && !GameSaver.loading)
        {
            StartCoroutine(LoadNextScene());
        }
    }
    IEnumerator LoadNextScene()
    {
        changingScene = true;
        Crossfade.current.StartFade();
        DialogController.main.closeBox();
        yield return new WaitForSeconds(1f);
        DisableMenuMusic();
        Player.controller.noFall = true;
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
        Player.controller.noFall = true;
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
        DialogController.main?.closeBox();
        yield return new WaitForSeconds(1f);
        DisableMenuMusic();
        if(Player.controller != null)
            Player.controller.noFall = true;
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
            CanvasManager.ShowHUD(true);
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
}