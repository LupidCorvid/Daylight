using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{

    public float lastStopTime = -100;
    float stopTimeDuration = 0;

    public static float regularTimeSpeed = 1;

    public float lastReductionAmount = 0.9f;

    float regularFixedDeltaTime;

    private void Awake()
    {
        regularTimeSpeed = Time.timeScale;
        //Time.fixedDeltaTime = regularTimeSpeed;
        regularFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Update()
    {
        if (PauseScreen.paused)
            return;

        if (stopTimeDuration == 0 || stopTimeDuration + lastStopTime < Time.time || ChangeScene.changingScene)
            Time.timeScale = regularTimeSpeed;
        else
        {
            float scalar = Mathf.Lerp(regularTimeSpeed - (lastReductionAmount * regularTimeSpeed), regularTimeSpeed, (Time.time - lastStopTime) / stopTimeDuration);
            Time.timeScale = Mathf.Clamp(scalar, 0, 1);
            Time.fixedDeltaTime = Mathf.Clamp(scalar, 0, 1) * regularFixedDeltaTime;
        }
            
    }

    public void StopTime(float amount = .45f, float length = .5f)
    {
        if (length >= stopTimeDuration - (Time.time - lastStopTime) || lastReductionAmount < amount || stopTimeDuration + lastStopTime < Time.time)
        {
            lastStopTime = Time.time;
            lastReductionAmount = amount;
            stopTimeDuration = length;
        }
    }
}
