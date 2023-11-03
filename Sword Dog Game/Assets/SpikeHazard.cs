using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity hit = collision.GetComponent<Entity>();

        if(hit.gameObject.tag == "Player")
        {
            hit.TakeDamage(damage, null);

            hit.gameObject.transform.position = SafeSpot.lastSafeSpot.respawnLocation.position;
            Camera.main.transform.position = new Vector3(SafeSpot.lastSafeSpot.respawnLocation.position.x, SafeSpot.lastSafeSpot.respawnLocation.position.y, Camera.main.transform.position.z);
        }
        else
        {
            //Kill non-player entities instantly (since they won't be respawned out of it)
            hit.TakeDamage(50, null);

        }
    }
}
