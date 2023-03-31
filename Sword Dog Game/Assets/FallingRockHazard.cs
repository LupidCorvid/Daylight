using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRockHazard : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage = 3;
    public bool scaleWithSpeed;
    public SpriteRenderer rend;


    public float lastOnScreen;
    float lifeTimeOffScreen = 2;

    bool dropped = false;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rend.isVisible)
        {
            lastOnScreen = Time.time;
        }

        if (Time.time > lastOnScreen + lifeTimeOffScreen && dropped && rb.velocity.magnitude < 1.5f)
            Destroy(rb.gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        testCollision(collision.collider);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        testCollision(collision);
    }

    public void testCollision(Collider2D collision)
    {
        PlayerHealth hit = collision.transform.GetComponent<PlayerHealth>();
        if (hit != null)
        {
            if (scaleWithSpeed)
                hit.TakeDamage((int)(rb.velocity.magnitude * damage));
            else
                hit.TakeDamage(damage);
        }

        EnemyBase enemyHit = collision.transform.GetComponent<EnemyBase>();
        if (enemyHit != null)
        {

            if (scaleWithSpeed && ((int )rb.velocity.magnitude * damage) > 0)
                enemyHit.TakeDamage((int)(rb.velocity.magnitude * damage));
            else
                enemyHit.TakeDamage(damage);
        }
    }


    public void Drop()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.down * .01f;
        dropped = true;
    }
}
