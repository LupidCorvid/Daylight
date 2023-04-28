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
    private Animator crossfade;
    public static bool changingScene = false;
    public bool noFall = false;
    public static Action clearCollisions, clearInteractables;

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
        
    }

    public static void ChangeSceneMinimal(string scene)
    {
        changingScene = true;
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        GameObject.Destroy(eventSystem?.gameObject);
        SceneHelper.LoadScene(scene);
        clearCollisions?.Invoke();
        clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        //DialogController.closedAnimator = true;

    }

}