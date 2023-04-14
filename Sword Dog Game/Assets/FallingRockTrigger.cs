using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRockTrigger : MonoBehaviour
{
    public FallingRockHazard hazard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.GetComponent<PlayerHealth>() != null)
        //{
        //    hazard.Drop();
        //}
        Entity hit = collision.GetComponent<Entity>();
        if(hit != null && (hit.allies & Entity.Team.Player) > 0)
        {
            hazard.Drop();
        }
    }
}
