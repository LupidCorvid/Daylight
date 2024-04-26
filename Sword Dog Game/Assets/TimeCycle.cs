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

    public float HumidityVal = 0;

    private float rainScalar;

    public float RainStartTime;

    public float windVal;

    public float roomBaseWind = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRain();
        UpdateWind();
    }

    private void Update()
    {
        Player.instance.rain.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //UpdateWind();
        if (lastWeatherUpdate + 1 < Time.time)
        {
            UpdateRain();
            
        }
    }

    public bool CheckIsRaining()
    {
        //Debug.Log("Updating weather " +  Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) + " Input: " + ((roomLocation.x + volatilityScalar * GameTime)));
        HumidityVal = Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime));

        if (Raining)
            return Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) > stopRainReq;
        else
            return Mathf.PerlinNoise1D((roomLocation.x + volatilityScalar * GameTime)) > neededHumidity;

    }

    public void SetRainState()
    {
        var emitter = Player.instance.rain.emission;
        if (Raining)
            emitter.rateOverTime = 400 * ((HumidityVal - rainScalar) /(1 - rainScalar));
        else
            emitter.rateOverTime = 0;
        
    }

    public void UpdateRain()
    {

        if (!Raining && CheckIsRaining())
            RainStartTime = Time.time;

        Raining = CheckIsRaining();
        lastWeatherUpdate = Time.time;

        rainScalar = Mathf.Lerp(neededHumidity, stopRainReq, (Time.time - RainStartTime) / 5);

        SetRainState();
    }

    public void UpdateWind()
    {
        SceneWindSetter.currActive.strengths.x = roomBaseWind + 10 * (Mathf.PerlinNoise(roomLocation.x + GameTime * volatilityScalar/100f, 100) - .5f)/.5f * 2;
        windVal = SceneWindSetter.currActive.strengths.x;
    }
}
