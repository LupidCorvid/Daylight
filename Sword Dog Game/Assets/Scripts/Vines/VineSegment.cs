using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSegment : MonoBehaviour
{
    public Rigidbody2D rb;

    public HingeJoint2D connection;


    public float windStrength = 1;
    public float windSpeed = 1;
    //Lower wind volatility means objects near eachother have similar swaying motion
    public float windVolatility = 0.2f;

    public float reactiveResponseScalar = 1;

    //For use with SceneWindSetter
    public static float sceneStrengthScalar = 1;
    public static float sceneSpeedScalar = 1;
    public static float sceneVolatilityScalar = 1;

    private void Awake()
    {
        fillInComponents();        
    }

    public void fillInComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        connection = GetComponent<HingeJoint2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((int)Mathf.Pow(2, collision.gameObject.layer) & LayerMask.GetMask("TerrainFX", "Utility")) == 0)
        {
            if (collision.GetComponent<Rigidbody2D>() != null)
                rb.velocity += collision.GetComponent<Rigidbody2D>().velocity * Time.deltaTime * reactiveResponseScalar;
        }
    }
    public void FixedUpdate()
    {
        //Culling
        if (Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > 20 * Camera.main.orthographicSize / 6 || !SettingsManager.currentSettings.GrassSway)
            return;

        //Wind direction makes it so that wind rolls in the same direction as things are bending
        //int windDirection = (windStrength * sceneSpeedScalar > 0 ? -1 : 1);
        //rb.velocity += (Vector2.right * Mathf.PerlinNoise(((Time.time * windSpeed * windDirection * sceneSpeedScalar) + (transform.position.x)) * windVolatility * sceneVolatilityScalar, 0) * Time.deltaTime * windStrength * sceneStrengthScalar)/rb.mass;
        rb.velocity += (Vector2.right * SwayEffect.getWindEffect(transform.position.x,windSpeed,windVolatility,windStrength)) / rb.mass;
    }
}
