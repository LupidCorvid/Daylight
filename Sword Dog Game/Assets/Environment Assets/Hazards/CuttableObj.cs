using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableObj : MonoBehaviour
{
    public ParticleSystem targParticleSystem;
    public GameObject sprite;
    public Collider2D blockingCollider;
    public Collider2D hittableCollider;

    bool markedToDelete = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<SwordFollow>()?.pmScript?.attacking == true && !markedToDelete)
        {
            targParticleSystem.Emit(10);
            Destroy(sprite);
            Destroy(blockingCollider.gameObject);
            hittableCollider.enabled = false;
            markedToDelete = true;
        }
    }

    private void Update()
    {
        if (markedToDelete && targParticleSystem.particleCount <= 0)
            Destroy(gameObject);
    }
}
