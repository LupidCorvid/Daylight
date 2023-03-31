using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonIvyHazard : MonoBehaviour
{
    public float damage = 1;

    public static List<TargetHit> hitTargets = new List<TargetHit>();
    public static List<PlayerHealth> hitThisFrame = new List<PlayerHealth>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealth hit = collision.GetComponent<PlayerHealth>();
        if (hit == null)
            return;

        if (hitThisFrame?.Contains(hit) == false)
            hitThisFrame.Add(hit);
    }

    public void LateUpdate()
    {
        foreach (PlayerHealth hit in hitThisFrame)
        {
            int foundIndex = hitTargets.FindIndex((x) => x.target == hit);
            if (foundIndex == -1)
            {
                foundIndex = hitTargets.Count;
                hitTargets.Add(new TargetHit(hit, 0));
            }

            hitTargets[foundIndex].damage += damage * Time.fixedDeltaTime;
            hitTargets[foundIndex].hitThisFrame = true;
        }

        for (int i = hitTargets.Count - 1; i >= 0; i--)
        {
            if (!hitTargets[i].hitThisFrame)
                hitTargets[i].damage -= damage * Time.fixedDeltaTime;
            hitTargets[i].hitThisFrame = false;

            if (hitTargets[i].damage >= 1)
            {
                hitTargets[i].target.TakeDamage((int)hitTargets[i].damage / 1);
                hitTargets[i].damage %= 1;
            }


            if (hitTargets[i].damage <= 0)
                hitTargets.RemoveAt(i);

        }

        hitThisFrame.Clear();
    }


    public class TargetHit
    {
        public PlayerHealth target;
        public float damage;
        public bool hitThisFrame = false;


        public TargetHit(PlayerHealth target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }
    }
}
