using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script detects when the sword hits an enemy

public class SwordTip : MonoBehaviour
{
    public SwordFollow sword;

    public int damage = 1;

    public float knockback = 0;

    public List<Collider2D> sameAttackCollisions = new List<Collider2D>();

    public Animator swordAnimator;

    public Entity user;

    [SerializeField] GameObject slashFx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearCollisions()
    {
        sameAttackCollisions.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (PlayerHealth.dead && other.gameObject.CompareTag("Ground"))
        {
            sword.Freeze();
            return;
        }
        if (sword.pmScript == null)
            sword.pmScript = Player.controller;

        if (sword?.pmScript?.attacking == false)
            return;

        if (sameAttackCollisions.Contains(other))
            return;
        sameAttackCollisions.Add(other);


        EnemyBase enemy = other.GetComponent<EnemyBase>();
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();

        enemy?.TakeDamage(damage, user);
        if(enemy != null)
            Instantiate(slashFx, enemy.transform.position, Quaternion.identity); //Spawn slash effect

        if (otherRb != null)
        {
            if(otherRb.bodyType != RigidbodyType2D.Static && otherRb != sword.pmScript.rb)
                otherRb.AddForce(((Vector2)(other.transform.position - sword.pmScript.transform.position).normalized) * knockback * (Mathf.Clamp(sword.rb.velocity.magnitude, 0, 2)));
        }
    }
}
