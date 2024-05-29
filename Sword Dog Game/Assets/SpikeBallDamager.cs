using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBallDamager : MonoBehaviour
{
    public PassiveMover enemyBase;
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

        if (hit != null)
        {
            if (lastDamage + damageCooldown < Time.time && enemyBase.GetIfEnemies(hit) && (!requireMovement || (collision.GetComponent<Rigidbody2D>().velocity.magnitude > .1f)))
            {
                hit.TakeDamage(enemyBase.attackDamage, null);
                lastDamage = Time.time;
            }
        }
    }
}
