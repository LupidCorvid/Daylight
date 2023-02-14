using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionsTracker : MonoBehaviour
{
    public List<Collider2D> collidersInContact = new List<Collider2D>();

    public List<Collider2D> triggersInContact = new List<Collider2D>();
    public event Action<Collider2D> triggerEnter;
    public event Action<Collision2D> colliderEnter;

    public Collider2D cldr;

    private void Awake()
    {
        cldr = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeScene.clearCollisions += ClearAll;
        
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collidersInContact.Add(collision.collider);
        colliderEnter?.Invoke(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collidersInContact.Remove(collision.collider);
    }
    private void ClearAll()
    {
        collidersInContact.Clear();
        triggersInContact.Clear();
    }

    public void clean()
    {
        for(int i = collidersInContact.Count - 1; i >=0; i--)
        {
            if (collidersInContact[i] == null)
                collidersInContact.RemoveAt(i);
        }
        for (int i = triggersInContact.Count - 1; i >= 0; i--)
        {
            if (triggersInContact[i] == null)
                triggersInContact.RemoveAt(i);
        }
    }
}
