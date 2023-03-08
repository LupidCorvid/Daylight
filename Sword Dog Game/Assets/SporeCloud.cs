using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeCloud : MonoBehaviour
{

    public float SpawnedTime;
    public float LifeTime = 8f;
    public ParticleSystem pSys;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnedTime + LifeTime < Time.time)
            Destroy(gameObject);
    }
}
