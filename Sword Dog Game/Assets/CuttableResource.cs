using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableResource : MonoBehaviour
{
    public int dropItemId;
    public int dropAmount = 1; //Maybe randomize

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponentInChildren<SwordFollow>() != null)
        {
            Utilities.main.SpawnLooseItem(ItemDatabase.main.getItemFromId(dropItemId, dropAmount), transform.position, new Vector2(Random.Range(-.5f, .5f), Random.Range(-.25f, .25f)));
            Destroy(gameObject);
        }
    }
}
