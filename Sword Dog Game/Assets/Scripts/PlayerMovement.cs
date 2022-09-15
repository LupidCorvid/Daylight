using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float speed = 2f;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float targetVelocity = Mathf.Lerp(rb.velocity.x, moveX * speed, 0.1f);
        rb.velocity = new Vector3(targetVelocity, rb.velocity.y, 0);
        GetComponent<Animator>().SetFloat("speed", targetVelocity / speed);
        GetComponent<Animator>().SetFloat("absSpeed", Mathf.Abs(targetVelocity / speed));
    }
}
