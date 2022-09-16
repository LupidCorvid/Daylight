using UnityEngine;
using UnityEngine.SceneManagement;

// https://forum.unity.com/threads/application-loadlevel-resets-input-getaxis-work-arounds.118511/#post-7003463
public static partial class SceneHelper
{
    // @kurtdekker - to load next scene without resetting Input axes
    public static void LoadScene(string SceneNameToLoad)
    {
        PendingPreviousScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += ActivatorAndUnloader;
        SceneManager.LoadScene(SceneNameToLoad, LoadSceneMode.Additive);
    }

    static string PendingPreviousScene;
    static void ActivatorAndUnloader(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ActivatorAndUnloader;
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(PendingPreviousScene);
    }
}