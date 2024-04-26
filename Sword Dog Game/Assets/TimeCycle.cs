using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCycle : MonoBehaviour
{
    public int GameStartTime = 0;

    public int GameTime
    {
        get { return GameStartTime + (int)Time.time; }
    }

    public Vector2 roomLocation = default;

    public bool Raining = false;

    public float neededHumidity = 0.9f;
    public float stopRainReq = 0.75f;

    private float lastWeatherUpdate = 0;

    public float volatilityScalar = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        Raining = CheckIsRaining();
        lastWeatherUpdate = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(lastWeatherUpdate + 1 < Time.time)
        {
            Raining = CheckIsRaining();
            lastWeatherUpdate = Time.time;
            
        }
    }

    public bool CheckIsRaining()
    {
        Debug.Log("Updating weather " +  Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) + " Input: " + ((roomLocation.x + volatilityScalar * GameTime)));
        
        if(Raining)
            return Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) > stopRainReq;
        else
            return Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) > neededHumidity;

    }
}
