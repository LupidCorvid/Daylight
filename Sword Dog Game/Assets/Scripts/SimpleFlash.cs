using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings

    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;
    [Tooltip("Other flashes to trigger when this one is triggered.")]
    [SerializeField] private List<SimpleFlash> alsoFlash = new List<SimpleFlash>();
    [Tooltip("Force using non-solid flash. For when a sprite may have color set to black by default.")]
    [SerializeField] private bool forceNonSolidFlash = false;
    #endregion
    #region Private Fields

    // The SpriteRenderer that should flash.
    [SerializeField] private SpriteRenderer spriteRenderer;

    // The material that was in use, when the script started.
    private Material originalMaterial;

    private Color originalColor, newColor;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    #endregion

    #endregion


    #region Methods

    #region Unity Callbacks

    void Start()
    {
        // Get the SpriteRenderer to be used,
        // alternatively you could set it from the inspector.
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the material that the SpriteRenderer uses, 
        // so we can switch back to it after the flash ended.
        originalMaterial = spriteRenderer.material;

        originalColor = spriteRenderer.color;
        newColor = flashMaterial.color;
    }

    #endregion

    public void Flash(float duration, int amount, bool solid = false)
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        if (solid && !forceNonSolidFlash)
            flashRoutine = StartCoroutine(SolidFlashRoutine(duration, amount));
        else
            flashRoutine = StartCoroutine(FlashRoutine(duration, amount));

        //Trigger any other flashes that are grouped with this one.
        foreach(SimpleFlash flash in alsoFlash)
        {
            //Prevent infinite loops by only calling flashes that don't loop back to this. An infinite loop is still possible with a 3 way connection
            if(!flash.alsoFlash.Contains(this))
                flash.Flash(duration, amount, solid);
        }
    }

    private IEnumerator FlashRoutine(float duration, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Swap to the flashMaterial.
            spriteRenderer.color = newColor;

            // Pause the execution of this function for "duration" seconds.
            yield return new WaitForSeconds(duration / amount / 2);

            // After the pause, swap back to the original material.
            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(duration / amount / 2);
        }

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }

    private IEnumerator SolidFlashRoutine(float duration, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Swap to the flashMaterial.
            spriteRenderer.material = flashMaterial;

            // Pause the execution of this function for "duration" seconds.
            yield return new WaitForSecondsRealtime(duration / amount / 2);

            // After the pause, swap back to the original material.
            spriteRenderer.material = originalMaterial;

            yield return new WaitForSecondsRealtime(duration / amount / 2);
        }

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }

    #endregion
}