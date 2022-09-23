using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionsTracker : MonoBehaviour
{
    public List<Collider2D> collidersInContact = new List<Collider2D>();

    public List<Collider2D> triggersInContact = new List<Collider2D>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggersInContact.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        triggersInContact.Remove(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collidersInContact.Add(collision.collider);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collidersInContact.Remove(collision.collider);
    }
}
