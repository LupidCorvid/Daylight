using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    [SerializeField] private float speed = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float targetVelocity = Mathf.Lerp(rb.velocity.x, moveX * speed, 0.1f);
        rb.velocity = new Vector3(targetVelocity, rb.velocity.y, 0);

        anim.SetFloat("speed", targetVelocity / speed);
        anim.SetFloat("absSpeed", Mathf.Abs(targetVelocity / speed));
    }
}
