using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        //Right
        if (Input.GetKey(KeyCode.D))
        {
            //TODO: cap velocity
            rb.AddForce(transform.right * 2);
        }

        //Slow down when you let go of key
        if (Input.GetKeyUp(KeyCode.D))
        {
            //TODO: Slow down, dont immediately stop
            rb.velocity = Vector2.zero;
        }

        //Left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.right * -2);
        }

        //Slow down when you let go of key
        if (Input.GetKeyUp(KeyCode.A))
        {
            //TODO: Slow down, dont immediately stop
            rb.velocity = Vector2.zero;
        }
    }
}
