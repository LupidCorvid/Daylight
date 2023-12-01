using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private Animation anim;
    public bool wobbling;

    // 0 = empty, 1 = half, 2 = full
    public Sprite[] normalSprites, damagedSprites;

    private AnimationClip wobble, blink;
    public static float wobbleSpeed = 0.3f;
    private static bool flipWobble = false;
    public static int MinWobbleHealth = 3;
    public float offset;
    private float wobbleIntensity = 0f, targetIntensity = 0f;
    public int type = 2;
    public bool damaged = false;
    private Coroutine blinkRoutine, flashRoutine;

    private void Start()
    {
        // create a new AnimationClip
        wobble = new AnimationClip();
        wobble.name = "Wobble";
        wobble.legacy = true;

        // create a curve to move the GameObject and assign to the clip
        Keyframe[] keys;
        keys = new Keyframe[5];
        for (int i = 0; i < 5; i++)
        {
            keys[i] = new Keyframe(wobbleSpeed * i/4f, Mathf.Sin(i/2f * Mathf.PI) * (flipWobble ? -1 : 1));
        }
        flipWobble = !flipWobble;
        
        for (int i = 1; i <= 2; i++){
            AnimationEvent stop = new AnimationEvent();
            stop.functionName = "CheckStopWobble";
            stop.time = wobbleSpeed / i;
            stop.objectReferenceParameter = this;
            wobble.AddEvent(stop);
        }

        AnimationCurve wobbleCurve = new AnimationCurve(keys);
        wobble.SetCurve("", typeof(Heart), "offset", wobbleCurve);
        wobble.wrapMode = WrapMode.Loop;
        anim.AddClip(wobble, wobble.name);
    }

    public void SetSprite(int newType, bool isDamaged)
    {
        damaged = isDamaged;

        int prevType = type;
        type = Mathf.Clamp(newType, 0, 2);
        if (type < prevType && blinkRoutine == null)
            blinkRoutine = StartCoroutine(Blink(prevType, 1f, 3));

        else if (damaged && flashRoutine == null)
            flashRoutine = StartCoroutine(DamageFlash(1f, 3));
        else
            GetComponent<Image>().sprite = normalSprites[type];
    }

    public void Wobble(float intensity)
    {
        targetIntensity = intensity;
        if (!wobbling)
        {
            anim.Play(wobble.name);
            wobbling = true;
            wobbleIntensity = intensity;
        }
    }

    public void StopWobble()
    {
        anim.Stop();
        wobbling = false;
    }

    public void CheckStopWobble()
    {
        if (!wobbling)
            StopWobble();
    }

    private IEnumerator DamageFlash(float duration, int amount)
    {
        Image image = GetComponent<Image>();
        for (int i = 0; i < amount; i++)
        {
            image.sprite = damagedSprites[type];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
            image.sprite = normalSprites[type];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
        }

        flashRoutine = null;
    }

    private IEnumerator Blink(int prevType, float duration, int amount)
    {
        Image image = GetComponent<Image>();
        for (int i = 0; i < amount; i++)
        {
            image.sprite = damagedSprites[prevType];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
            image.sprite = normalSprites[type];
            yield return new WaitForSecondsRealtime(duration / amount / 2);
        }

        // Set the routine to null, signaling that it's finished.
        blinkRoutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) Wobble(3f);
        if (Input.GetKeyDown(KeyCode.Y)) wobbling = false;

        if (!wobbling)
            targetIntensity = 0f;

        wobbleIntensity = Mathf.Lerp(wobbleIntensity, targetIntensity, 0.01f);
        
        transform.localPosition = new Vector2(transform.localPosition.x, wobbleIntensity * offset);
    }
}
