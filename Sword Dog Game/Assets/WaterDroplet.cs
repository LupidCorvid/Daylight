using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDroplet : MonoBehaviour
{
    public float startTime;
    public float minLifeTime;
    public Rigidbody2D rb;
    public float rotationOffset;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.right, rb.velocity) + rotationOffset);

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(startTime + minLifeTime < Time.time)
            Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (startTime + minLifeTime < Time.time)
            Destroy(gameObject);
    }
}
