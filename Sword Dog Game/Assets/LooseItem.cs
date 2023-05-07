using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = TempObjectsHolder.main.FindSprite(item.sprite);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Entity hitEntity = collision.GetComponent<Entity>();

        if(hitEntity?.getAssociatedInventory() != null)
        {
            hitEntity?.getAssociatedInventory().AddItem(item);
            if (item.quantity <= 0)
                Destroy(transform.parent.gameObject);
        }
    }
}
