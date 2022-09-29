using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionsTracker : MonoBehaviour
{
    public List<Collider2D> collidersInContact = new List<Collider2D>();

    public List<Collider2D> triggersInContact = new List<Collider2D>();

    public event Action<Collider2D> triggerEnter;
    public event Action<Collider2D> triggerLeave;

    public event Action<Collision2D> collisionEnter;
    public event Action<Collision2D> collisionLeave;
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
        triggerEnter?.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        triggersInContact.Remove(collision);
        triggerLeave?.Invoke(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collidersInContact.Add(collision.collider);
        collisionEnter?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collidersInContact.Remove(collision.collider);
        collisionLeave?.Invoke(collision);
    }
}
