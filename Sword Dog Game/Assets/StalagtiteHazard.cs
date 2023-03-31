using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagtiteHazard : MonoBehaviour
{
    public int damage = 1;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth hit = collision.GetComponent<PlayerHealth>();
        if(hit != null && rb.velocity.magnitude > .01f)
        {
            hit.TakeDamage(damage);
            
        }

        EnemyBase enemyHit = collision.GetComponent<EnemyBase>();
        if(enemyHit != null && rb.velocity.magnitude > .01f)
        {
            enemyHit.TakeDamage(damage);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Drop()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.down * .01f;
    }
}
