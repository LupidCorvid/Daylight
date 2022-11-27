using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandParticle : MonoBehaviour
{
    Vector2 velocity;

    public float lifeTime = 2;
    public float spawnTime;

    public float windSpeed;
    public float windVolatility;
    public Vector2 windStrength;

    private Vector2 speedOffsets;

    public SpriteRenderer spriteRend;

    float startingAlpha = 1;

    //Maybe make it fade out over time?
    private void Awake()
    {
        spawnTime = Time.time;
        speedOffsets = new Vector2(Random.Range(.75f, 1.25f), Random.Range(.75f, 1.25f));
        startingAlpha = spriteRend.color.a;
    }

    // Update is called once per frame
    void Update()
    {

        if (spawnTime + lifeTime < Time.time)
        {
            Destroy(gameObject);
            return;
        }
        transform.position += (Vector3)velocity;

        velocity *= 0.5f * Time.deltaTime;

        int windDirection = (windStrength.x > 0 ? -1 : 1);
        velocity += (Vector2.right * Mathf.PerlinNoise(((Time.time * windSpeed * windDirection) + (transform.position.x)) * windVolatility, 0) * Time.deltaTime * (windStrength.x * speedOffsets.x));
        velocity += (Vector2.up * Mathf.PerlinNoise(Time.time, 100)) * Time.deltaTime * windStrength.y * speedOffsets.y;


        spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, Mathf.Lerp(startingAlpha, 0, (Time.time - spawnTime)/lifeTime));

    }
}
