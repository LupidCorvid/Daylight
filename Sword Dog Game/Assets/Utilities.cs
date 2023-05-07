using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static Utilities main;
    public GameObject looseItemPrefab;
    public GameObject BurstFieldPrefab;
    // Start is called before the first frame update
    void Start()
    {
        main ??= this;
    }

    public void SpawnLooseItem(Item item, Vector2 location, Vector2 velocities = new Vector2())
    {
        GameObject spawnedItem = Instantiate(looseItemPrefab, location, Quaternion.identity, TempObjectsHolder.asTransform);
        spawnedItem.GetComponent<Rigidbody2D>().velocity = velocities;
        LooseItem looseItem = spawnedItem.GetComponentInChildren<LooseItem>();
        looseItem.item = item;
    }

    public void SpawnWindBurstEffect(Vector2 location, float size, float lifeTime, float strength)
    {
        GameObject spawnedObject = Instantiate(BurstFieldPrefab, location, Quaternion.identity, TempObjectsHolder.asTransform);
        WindBurstEffect effect = spawnedObject.GetComponent<WindBurstEffect>();
        effect.radius = size;
        effect.lifeTime = lifeTime;
        effect.strength = strength;
    }
}
