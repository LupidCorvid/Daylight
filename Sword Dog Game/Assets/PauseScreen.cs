using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScreen : MonoBehaviour
{
    public static bool paused = false;
    public static bool canPause = true;

    CanvasGroup pauseMenuGroup;

    public UnityEngine.UI.Button resumeButton;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {

        if ((Input.GetAxisRaw("Vertical") != 0) && EventSystem.current.currentSelectedGameObject == null && paused)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }

        //Temp implementation. Should work to escape menus and check if not in menus first
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (!paused)
            {
                Pause();
            }
            else
            {
                unPause();
            }
        }
    }
    public void Pause()
    {
        if (!canPause)
            return;
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        paused = true;
        Time.timeScale = 0;
        foreach (AudioSource source in sources)
        {
            source.Pause();
        }
        pauseMenuGroup.alpha = 1;
        pauseMenuGroup.blocksRaycasts = true;
        pauseMenuGroup.interactable = true;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void unPause()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        paused = false;
        Time.timeScale = 1;
        foreach (AudioSource source in sources)
        {
            source.UnPause();
        }

        pauseMenuGroup.alpha = 0;
        pauseMenuGroup.blocksRaycasts = false;
        pauseMenuGroup.interactable = false;
    }

    public void QuitToTitle()
    {
        GameSaver.main.SaveGame();
        unPause();
        //SceneHelper.LoadScene("Main Menu");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main Menu"));
        Destroy(PlayerMovement.controller.gameObject);

        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        CanvasManager.HideHUD();
        //SceneManager.UnloadSceneAsync("DontDestroyOnLoad");
        
    }
}
