using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDroplet : MonoBehaviour
{
    float startTime = float.NaN;
    public float lifeTime = 0.25f;
    public Rigidbody2D rb;
    public float rotationOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.right, rb.velocity) + rotationOffset);

        if (!float.IsNaN(startTime) && startTime + lifeTime < Time.time)
            Destroy(gameObject);

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //maybe make this start a .25s timer to delete instead
        if (float.IsNaN(startTime))
            startTime = Time.time;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (float.IsNaN(startTime))
            startTime = Time.time;
    }
}
