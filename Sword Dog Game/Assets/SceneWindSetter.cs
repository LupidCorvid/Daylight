using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWindSetter : MonoBehaviour
{
    public Vector2 strengths;
    public float volatility;
    public float speed;

    public WindZone windZone;

    public bool linkStrengthSpeedVelo = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(linkStrengthSpeedVelo)
        {
            speed = 1 + ((Mathf.Abs(strengths.x) - 1) / 4f);
            volatility = Mathf.Abs(strengths.x) * 1.5f;
        }
        int direction = (strengths.x >= 0 ? 1 : -1);

        LeavesSway.sceneIntensityScalar = 1 + (Mathf.Abs(strengths.x) - 1)/5f;
        LeavesSway.sceneSpeedScalar = speed;
        LeavesSway.sceneVolatilityScalar = volatility;

        if (Mathf.Abs(strengths.x) > 1)
            SwayEffect.sceneStrengthScalar = (1 + Mathf.Clamp((Mathf.Abs(strengths.x) - 1) / 10f, -1, 1)) * direction;
        else
            SwayEffect.sceneStrengthScalar = strengths.x;
        //if (strengths.x == 0)
        //    SwayEffect.sceneStrengthScalar = 0;

        SwayEffect.sceneSpeedScalar = /*speed*/1 + (Mathf.Abs(strengths.x) - 1) * .25f;
        if (SwayEffect.sceneSpeedScalar > 3)
            SwayEffect.sceneSpeedScalar = 3;
        SwayEffect.sceneVolatilityScalar = 1 + (volatility - 1)/100f;

        VineSegment.sceneStrengthScalar = strengths.x;
        VineSegment.sceneSpeedScalar = speed;
        VineSegment.sceneVolatilityScalar = 1 + (volatility - 1) / 100f;

        int neg = -1;
        if (strengths.y < 0)
            neg = 1;
        windZone.transform.rotation = Quaternion.Euler(Vector3.Angle(Vector3.right, strengths) * neg, 90, 0);
        windZone.windMain = strengths.magnitude;
        windZone.windTurbulence = volatility;
    }
}
