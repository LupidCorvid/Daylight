using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeCloud : MonoBehaviour
{

    public float SpawnedTime;
    public float LifeTime = 8f;
    public ParticleSystem pSys;

    public BugShroom mainEnemy;
    public BugShroomAI ai
    {
        get
        {
            if (mainEnemy == null)
                return null;
            return (BugShroomAI)(mainEnemy.ai);
        }
    }

    public Dictionary<PlayerHealth, float> targetsHit = new Dictionary<PlayerHealth, float>();
    public List<PlayerHealth> hitThisFrame = new List<PlayerHealth>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnedTime + LifeTime < Time.time)
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealth hit = collision.GetComponent<PlayerHealth>();
        if (hit == null)
            return;

        if (ai?.hitThisFrame?.Contains(hit) == false)
            ai.hitThisFrame.Add(hit);

    }
}
