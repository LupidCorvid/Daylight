using System.Collections;

using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings

    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    #endregion
    #region Private Fields

    // The SpriteRenderer that should flash.
    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the material that the SpriteRenderer uses, 
        // so we can switch back to it after the flash ended.
        originalMaterial = spriteRenderer.material;

        originalColor = spriteRenderer.color;
        newColor = originalColor;
        newColor.a = 0.5f;
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
        if (solid)
            flashRoutine = StartCoroutine(SolidFlashRoutine(duration, amount));
        else
            flashRoutine = StartCoroutine(FlashRoutine(duration, amount));
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
            yield return new WaitForSeconds(duration / amount / 2);

            // After the pause, swap back to the original material.
            spriteRenderer.material = originalMaterial;

            yield return new WaitForSeconds(duration / amount / 2);
        }

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }

    #endregion
}