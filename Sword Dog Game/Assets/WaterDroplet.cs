using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDroplet : MonoBehaviour
{
    float startTime = float.NaN;
    public float lifeTime = 0.25f;
    public Rigidbody2D rb;
    public float rotationOffset;
    public float startAlpha;
    public SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        startAlpha = rend.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.right, rb.velocity) + rotationOffset);



        if (!float.IsNaN(startTime))
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, (1 - (Time.time - startTime) / lifeTime) * startAlpha);

            if(startTime + lifeTime < Time.time)
                Destroy(gameObject);
        }

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
