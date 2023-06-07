using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagtiteTrigger : MonoBehaviour
{
    public StalagtiteHazard hazard;

    BoxCollider2D cldr;
    // Start is called before the first frame update
    void Start()
    {
        cldr = GetComponent<BoxCollider2D>();
        Vector2 hitPoint = Physics2D.Raycast(transform.position, Vector2.down, 100, LayerMask.GetMask("Terrain")).point;
        cldr.offset = (hitPoint - (Vector2)transform.position)/2;
        cldr.size = new Vector2(2, ((Vector2)transform.position - hitPoint).y);

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
        //    Destroy(this);
        //}
        Entity hit = collision.GetComponent<Entity>();
        if(hit != null && (hit.allies & ITeam.Team.Player) > 0)
        {
            hazard.Drop();
            Destroy(this);
        }

    }
}
