using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEnabler : MonoBehaviour
{
    public Collider2D toEnable;


    private float startTime;
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTime + delay < Time.time)
        {
            toEnable.enabled = true;
            Destroy(this);
        }
    }
}
