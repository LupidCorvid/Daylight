using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummyScript : EnemyBase
{
    Animator dummyHitAnim;
    bool flag = false;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        dummyHitAnim = gameObject.GetComponent<Animator>();
        maxHealth = int.MaxValue;
        health = int.MaxValue;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ////dummyHitAnim.Play("test");
        //if (flag) dummyHitAnim.Play("dummyHit");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //flag = true;
        //print("hi");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //dummyHitAnim.Play("dummyStill"); //play after the anim is done actually
        //print("bye");
        //flag = false;
    }

    public override void TakeDamage(int amount)
    {
        dummyHitAnim.Play("dummyHitInitial");
    }
}
