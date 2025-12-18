using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DojoPositionTracker : MonoBehaviour
{
    private const float BLACKSMITH = -13.65262f;
    private const float DOJO = 26.50299f;

    // Update is called once per frame
    void Update()
    {
        float distance = (Player.instance.transform.position.x - BLACKSMITH) / (DOJO - BLACKSMITH) * 100f;
        AkUnitySoundEngine.SetRTPCValue("DojoBlacksmith", Mathf.Clamp(distance, 0, 100));
    }
}
