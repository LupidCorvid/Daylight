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
    public static bool changingScene;
    public static bool noFalling = false;
    public bool noFall = false;

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
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!changingScene && collision.gameObject.tag == "Player" && !PlayerMovement.controller.dead && !PlayerMovement.controller.resetting && !GameSaver.loading)
        {
            StartCoroutine(LoadNextScene());
        }
    }
    IEnumerator LoadNextScene()
    {
        changingScene = true;
        noFalling = noFall;
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(0.9f);
        // if (SceneManager.GetActiveScene().name == "Tutorial" && scene == "1stScene")
        // {
        //     Player.controller.initialFall = true;
        // }
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            GameObject.Destroy(eventSystem.gameObject);
        }
        SceneHelper.LoadScene(scene);
        SpawnManager.spawningAt = spawn;
        changingScene = false;
    }

}