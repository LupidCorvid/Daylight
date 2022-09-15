using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    bool facingRight;
    [SerializeField] private float speed = 2f;

    void Start()
    {
        facingRight = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        if ((moveX < 0 && facingRight) || (moveX > 0 && !facingRight))
        {
            Flip();
        }

        float targetVelocity = Mathf.Lerp(rb.velocity.x, moveX * speed, 0.1f);
        rb.velocity = new Vector3(targetVelocity, rb.velocity.y, 0);

        anim.SetFloat("speed", Mathf.Abs(targetVelocity / speed));
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
    }
}
