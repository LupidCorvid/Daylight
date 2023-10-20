using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScreen : MonoBehaviour
{
    public static bool paused = false;
    public static bool canPause = true;
    public static bool quit = false;

    CanvasGroup pauseMenuGroup;

    public CanvasGroup pauseContainerGroup;

    public UnityEngine.UI.Button resumeButton, yesButton;

    public CanvasGroup quitPrompt;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuGroup = GetComponent<CanvasGroup>();
        //Debug.Log(Camera.main.pixelWidth + "  " + Camera.main.pixelHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.inputs?.actions["Move"].ReadValue<Vector2>().y != 0 && EventSystem.current.currentSelectedGameObject == null && paused)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }

        //Temp implementation. Should work to escape menus and check if not in menus first
        if (PlayerMovement.inputs != null && PlayerMovement.inputs.actions["Pause"].WasPressedThisFrame())
        {
            if(PlayerMenuManager.open)
            {
                PlayerMenuManager.main.closeMenu();
                return;
            }

            if(MenuManager.inMenu)
            {
                MenuManager.main.closeMenu();
                return;
            }

            if (ChangeScene.changingScene || GameSaver.loading || !Crossfade.over)
            {
                return;
            }

            if (PlayerMovement.controller != null && PlayerMovement.controller.resetting)
            {
                return;
            }
            
            if(quitPrompt.alpha == 1)
            {
                closePrompt();
            }

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
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        pauseContainerGroup.alpha = 1;
        pauseContainerGroup.blocksRaycasts = true;
        pauseContainerGroup.interactable = true;

        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        // TODO super sloppy but re-pauses music + dialog speakers
        if (AudioManager.instance.paused)
            AudioManager.instance.PauseCurrent();
        if (DialogController.dialogOpen)
        {
            if (DialogController.speaker.pausedSpeak)
                DialogController.speaker.pauseSpeak();
            else if (!DialogController.speaker.speaking)
                DialogController.speaker.stopSpeak();
        }
            
        pauseMenuGroup.alpha = 0;
        pauseMenuGroup.blocksRaycasts = false;
        pauseMenuGroup.interactable = false;

        pauseContainerGroup.alpha = 0;
        pauseContainerGroup.blocksRaycasts = false;
        pauseContainerGroup.interactable = false;
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void PromptQuit()
    {
        quitPrompt.alpha = 1;
        quitPrompt.blocksRaycasts = true;
        quitPrompt.interactable = true;
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);

        pauseMenuGroup.alpha = 0;
        pauseMenuGroup.blocksRaycasts = false;
        pauseMenuGroup.interactable = false;
    }

    public void CancelPrompt()
    {
        closePrompt();
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        pauseMenuGroup.alpha = 1;
        pauseMenuGroup.blocksRaycasts = true;
        pauseMenuGroup.interactable = true;
    }

    public void closePrompt()
    {
        quitPrompt.alpha = 0;
        quitPrompt.blocksRaycasts = false;
        quitPrompt.interactable = false;
    }

    public void QuitToTitle()
    {
        StartCoroutine(BackToMenu());
    }

    public IEnumerator BackToMenu() {
        quit = true;
        GameSaver.main.SaveGame();
        PlayerMovement.created = false;
        SwordFollow.created = false;
        closePrompt();
        unPause();
        ChangeScene.changingScene = true;
        // CanvasManager.InstantHideHUD();
        AudioManager.instance.FadeOutCurrent();
        Crossfade.current.StartFade();
        DialogController.main.closeBox();
        yield return new WaitForSeconds(1f);
        CutsceneController.inCutscene = false;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        ChangeScene.clearCollisions?.Invoke();
        ChangeScene.clearInteractables?.Invoke();
        CutsceneController.ClearCutscenes();
        DialogController.closedAnimator = true;
        Crossfade.current.StopFade();
        quit = false;
    }

}
