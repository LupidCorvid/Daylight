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

    public Animator anim
    {
        get
        {
            return enemyBase.anim;
        }
        set
        {
            enemyBase.anim = value;
        }
    }

    public int attackDamage
    {
        get
        {
            return enemyBase.attackDamage;
        }
        set
        {
            enemyBase.attackDamage = value;
        }
    }

    public Transform target;

    public Rigidbody2D rb;

    public static List<Transform> possibleTargets = new List<Transform>();

    public float aggroRange
    {
        get
        {
            return enemyBase.aggroRange;
        }
        set
        {
            enemyBase.aggroRange = value;
        }
    }

    public float FindTargetsWaitTime = .5f;
    float lastTargetSearch = -100;

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
        if(target == null && lastTargetSearch + FindTargetsWaitTime < Time.time)
        {
            target = GetTarget();
            lastTargetSearch = Time.time;
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual Transform GetTarget()
    {
        foreach(Transform targetLocation in possibleTargets)
        {
            if (targetLocation == null || transform == null)
                continue;
            float distance = Vector2.Distance(targetLocation.position, transform.position);

            if(distance <= aggroRange && !Physics2D.Linecast(transform.position, targetLocation.position, LayerMask.GetMask("Terrain")))
            {
                FoundTarget(targetLocation);
                return targetLocation;
                
            }
        }
        return null;
    }

    public virtual void FoundTarget(Transform newTarget)
    {

    }

    public virtual void applyAttackDamage()
    {

    }

    public virtual void DamageBox(Vector2 location, Vector2 range)
    {
        Collider2D[] hits;
        hits = Physics2D.OverlapAreaAll(location - range, location + range, LayerMask.GetMask("Entity"));
        Debug.DrawLine(location - range, location + range, Color.cyan, .25f);
        Debug.DrawLine(location - range * new Vector2(-1, 1), location + range * new Vector2(-1, 1), Color.cyan, .25f);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }
}
