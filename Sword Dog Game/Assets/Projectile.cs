using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IParryable, ITeam
{
    public float lifeTime;
    float spawnedTime;
    public Rigidbody2D rb;

    private Vector2 priorParryVel;

    public bool canBeParried
    {
        get { return true; }
        set { }
    }

    private ITeam.Team _allies;
    private ITeam.Team _enemies;

    public ITeam.Team allies
    {
        get { return _allies; }
        set { _allies = value; }
    }

    public ITeam.Team enemies
    {
        get { return _enemies; }
        set { _enemies = value; }
    }



    // Start is called before the first frame update
    void Start()
    {
        spawnedTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedTime + lifeTime < Time.time)
            Destroy(gameObject);
    }

    public void Parried(SwordFollow by)
    {
        rb.velocity = priorParryVel * -1;
    }

    public void Blocked(SwordFollow by)
    {
        priorParryVel = rb.velocity;
        rb.velocity = Vector2.zero;
        _allies = by.pmScript.entityBase.allies;
        _enemies = by.pmScript.entityBase.enemies;
    }
}
