using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int attackDamage = 1;
    public float moveSpeed = 1;
    public float attackSpeed = 1;

    public int maxHealth = 10;
    private int _health = 10;
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value > maxHealth)
                _health = maxHealth;
            else
                _health = value;
            if (_health <= 0)
                die();
        }
    }

    public BaseAI ai;

    public SlopeAdjuster slopeChecker;

    public virtual void Start()
    {
        slopeChecker ??= GetComponent<SlopeAdjuster>();
        ai?.Start();
    }

    public virtual void Update()
    {
        ai?.Update();
    }

    public void die()
    {
        Destroy(gameObject);
    }
}
