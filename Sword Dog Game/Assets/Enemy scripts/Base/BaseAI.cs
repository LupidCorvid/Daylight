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
            enemyBase.moveSpeed.baseValue = value;
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
            enemyBase.attackSpeed.baseValue = value;
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
            enemyBase.attackDamage.baseValue = value;
        }
    }

    public bool attacking = false;

    public Entity targetEntity;

    public Transform target
    {
        get
        {
            if (targetEntity == null)
                return null;
            return targetEntity?.transform;
        }
    }

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
        if (enemyBase.stunned)
            return;

        if(lastTargetSearch + FindTargetsWaitTime < Time.time && (target == null || enemyBase.GetIfEnemies(targetEntity) == false))
        {
            if(targetEntity != null && enemyBase.GetIfEnemies(targetEntity) == false)
            {
                targetEntity = null;
            }

            targetEntity = GetTarget();
            lastTargetSearch = Time.time;
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual Entity GetTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aggroRange, LayerMask.GetMask("Entity"));
        List<Entity> possibleTargets = new List<Entity>();
        foreach(Collider2D collision in hits)
        {
            Entity foundEntity = collision.GetComponent<Entity>();
            if(foundEntity?.GetIfEnemies(enemyBase) == true)
            {
                possibleTargets.Add(foundEntity);
            }
        }

        Entity nearestTarget = null;
        float nearestDistance = Mathf.Infinity;
        foreach(Entity target in possibleTargets)
        {
            if (target?.transform == null || transform == null)
                continue;
            //float distance = Vector2.Distance(targetLocation.position, transform.position);
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < nearestDistance && !Physics2D.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")))
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }
        if(target != null)
            FoundTarget(nearestTarget);

        return nearestTarget;
    }

    public virtual void FoundTarget(Entity newTarget)
    {
        targetEntity = newTarget;
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
            //hit.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            Entity hitEntity = hit.GetComponent<Entity>();
            if(hitEntity?.GetIfEnemies(enemyBase) == true)
            {
                RaycastHit2D parryDetect = Physics2D.Linecast(hit.transform.position, location, LayerMask.GetMask("DamageArea"));
                Debug.DrawLine(hit.transform.position, location);
                SwordFollow parryingSword = parryDetect.collider?.GetComponent<SwordFollow>();
                if (parryingSword == null)
                    hitEntity.TakeDamage(attackDamage, enemyBase);
                else if (parryingSword.pmScript.pAttack.isParrying)
                {
                    //Sword was parried
                    //enemyBase.buffManager.stunned.Inflict(1, 1);
                    enemyBase.Parried();
                }
            }
        }
    }
}
