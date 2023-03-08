using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugShroomAttack : MonoBehaviour
{
    public GameObject sporeCloudPrefab;
    public int numSporeClouds = 10;
    public float SporeCloudRadius = 3;
    public float SporeCloudAngleRange = 360;


    public void Attack()
    {
        for(int i = 0; i < numSporeClouds; i++)
        {
            GameObject cloud = Instantiate(sporeCloudPrefab, transform.position, transform.rotation, TempObjectsHolder.asTransform);
            Rigidbody2D cloudPhys = cloud.GetComponent<Rigidbody2D>();
            float angle = Random.Range(0, SporeCloudAngleRange);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            cloudPhys.velocity = direction * Random.Range(.25f, SporeCloudRadius) * cloudPhys.drag;

            //Set speed of cloud
        }
    }
}
