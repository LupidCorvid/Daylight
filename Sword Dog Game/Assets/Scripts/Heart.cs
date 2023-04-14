using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    private Vector3 originalPosition;
    [SerializeField] private Animation anim;
    public float offset;
    public bool wobbling;

    // 0 = empty, 1 = half, 2 = full
    // TODO: Add torn sprite
    public Sprite[] sprites;

    private AnimationClip wobble, blink;
    public static float wobbleAmplitude = 3f, wobblePeriod = 0.3f;
    private static bool flipWobble = false;
    private bool wobbledOnce = false;
    public static int MinWobbleHealth = 2;

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
            keys[i] = new Keyframe(wobblePeriod * i/4f, Mathf.Sin(i/2f * Mathf.PI) * wobbleAmplitude * (flipWobble ? -1 : 1));
        }
        flipWobble = !flipWobble;
        
        for (int i = 1; i <= 2; i++){
            AnimationEvent stop = new AnimationEvent();
            stop.functionName = "CheckStopWobble";
            stop.time = wobblePeriod / i;
            stop.objectReferenceParameter = this;
            wobble.AddEvent(stop);
        }

        AnimationCurve wobbleCurve = new AnimationCurve(keys);
        wobble.SetCurve("", typeof(Heart), "offset", wobbleCurve);
        wobble.wrapMode = WrapMode.Loop;
        anim.AddClip(wobble, wobble.name);
    }

    public void SetSprite(int type)
    {
        GetComponent<Image>().sprite = sprites[Mathf.Clamp(type, 0, 2)];
    }

    public void Wobble()
    {
        if (!wobbling)
        {
            originalPosition = transform.position;
            anim.Play(wobble.name);
            wobbling = true;
            wobbledOnce = true;
        }
    }

    public void StopWobble()
    {
        anim.Stop(); // TODO figure out how to make this only stop the one clip
        wobbling = false;
    }

    public void ForceStopWobble()
    {
        StopWobble();
        offset = 0;
    }

    public void CheckStopWobble()
    {
        if (!wobbling)
            StopWobble();
    }

    public void Blink() {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) Wobble();
        if (Input.GetKeyDown(KeyCode.Y)) wobbling = false;

        if (wobbledOnce)
            transform.position = new Vector3(originalPosition.x, originalPosition.y + offset);

        if (!wobbling)
            anim["Wobble"].speed = Mathf.Lerp(anim["Wobble"].speed, 0.2f, 0.1f);
        else
            anim["Wobble"].speed = 1f;
    }

}
