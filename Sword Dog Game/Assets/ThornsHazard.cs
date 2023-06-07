using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsHazard : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;
    public bool requireMovement = true;


    float lastDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Entity hit = collision.GetComponent<Entity>();

        if(hit != null)
        {
            if (lastDamage + damageCooldown < Time.time && (!requireMovement || (collision.GetComponent<Rigidbody2D>().velocity.magnitude > .1f)))
            {
                hit.TakeDamage(damage, null);
                lastDamage = Time.time;
            }
        }


        //PlayerHealth hit = collision.GetComponent<PlayerHealth>();
        //if(hit != null)
        //{
        //    if(lastDamage + damageCooldown < Time.time && (!requireMovement || (collision.GetComponent<Rigidbody2D>().velocity.magnitude > .1f)))
        //    {
        //        hit.TakeDamage(damage);
        //        lastDamage = Time.time;
        //    }
        //}

        //EnemyBase enemyHit = collision.GetComponent<EnemyBase>();
        //if(enemyHit != null)
        //{
        //    if (lastDamage + damageCooldown < Time.time && (!requireMovement || (collision.GetComponent<Rigidbody2D>().velocity.magnitude > .1f)))
        //    {
        //        enemyHit.TakeDamage(damage);
        //        lastDamage = Time.time;
        //    }
        //}
    }
}
