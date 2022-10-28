using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime;
    float spawnedTime;
    // Start is called before the first frame update
    void Start()
    {
        spawnedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedTime + lifeTime < Time.time)
            Destroy(gameObject);
    }
}
