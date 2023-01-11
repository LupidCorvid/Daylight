using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public EnemyBase enemyBase;

    public BaseMovement movement;

    public Transform transform
    {
        get
        {
            return enemyBase.transform;
        }
    }

    public GameObject gameObject
    {
        get
        {
            return enemyBase.gameObject;
        }
    }

    public float moveSpeed
    {
        get
        {
            return enemyBase.moveSpeed;
        }
        set
        {
            enemyBase.moveSpeed = value;
        }

    }

    public float attackSpeed
    {
        get
        {
            return enemyBase.attackSpeed;
        }
        set
        {
            enemyBase.attackSpeed = value;
        }
    }

    public Transform target;

    public Rigidbody2D rb;

    public BaseAI(EnemyBase baseScript)
    {
        enemyBase = baseScript;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {

    }

}
