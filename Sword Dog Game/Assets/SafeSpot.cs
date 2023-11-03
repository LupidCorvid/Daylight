using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeSpot : MonoBehaviour
{
    public static SafeSpot lastSafeSpot;
    public Transform respawnLocation;

    //Only one of these should be set to true per scene, when a scene is entered this is set to the initial one
    public bool DefaultSafeSpot = false;

    // Start is called before the first frame update
    void Start()
    {
        if (DefaultSafeSpot)
            lastSafeSpot = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            lastSafeSpot = this;
        }
    }
}
