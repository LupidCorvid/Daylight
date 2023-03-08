using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeCloud : MonoBehaviour
{

    public float SpawnedTime;
    public float LifeTime = 5f;

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
