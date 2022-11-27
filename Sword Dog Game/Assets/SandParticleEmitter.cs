using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandParticleEmitter : MonoBehaviour
{
    public float numPerSecond = 10;

    float lastStart = 0;

    public GameObject sandParticle;



    public float windSpeed;
    public float windVolatility;
    public Vector2 windStrength;

    public float particleLifetime = 2;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastStart + 1/numPerSecond < Time.time)
        {
            lastStart = Time.time;
            GameObject addedParticle = Instantiate(sandParticle, transform.position, transform.rotation, transform);
            SandParticle particle = addedParticle.GetComponent<SandParticle>();

            particle.spawnTime = Time.time;
            particle.windSpeed = windSpeed;
            particle.windVolatility = windVolatility;
            particle.windStrength = windStrength;
            particle.lifeTime = particleLifetime;
        }
    }
}
