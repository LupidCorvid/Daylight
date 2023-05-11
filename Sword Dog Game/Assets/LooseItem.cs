using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem : MonoBehaviour
{
    public Item item;
    public bool collected = false;
    SpriteRenderer rend;
    float animSpeed = 2;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = TempObjectsHolder.main.FindSprite(item.sprite);
    }

    public void Update()
    {
        if (collected)
            collectAnim();
    }

    public void collectAnim()
    {
        transform.position += Vector3.up * Time.deltaTime * animSpeed;
        rend.color =  new Color(rend.color.r, rend.color.g, rend.color.b, rend.color.a - animSpeed * Time.deltaTime);
        if (rend.color.a <= 0)
            Destroy(transform.parent.gameObject);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Entity hitEntity = collision.GetComponent<Entity>();

        if(hitEntity?.getAssociatedInventory() != null)
        {
            hitEntity?.getAssociatedInventory().AddItem(item);
            if (item.quantity <= 0)
                //Destroy(transform.parent.gameObject);
                collected = true;
        }
    }
}
