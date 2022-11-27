using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTip : MonoBehaviour
{
    public SwordFollow sword;

    public int damage = 1;

    public float knockback = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            // TODO do damage here
        }
        else if (other.gameObject.tag == "Ground")
        {
            sword.Freeze();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!sword.pmScript.attacking)
            return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        if(otherRb != null)
        {
            otherRb.velocity += ((Vector2)(other.transform.position - transform.position).normalized) * knockback * (Mathf.Clamp(sword.rb.velocity.magnitude, 0, 2));
        }
    }
}
