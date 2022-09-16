using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    bool facingRight, trotting;
    float moveX, targetVelocity;
    int stepDirection;
    [SerializeField] private float speed = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stepDirection = 1;
        facingRight = true;
    }
    
    void Update()
    {
        // grab movement input from horizontal axis
        moveX = Input.GetAxisRaw("Horizontal");

        // fix input spam breaking trot state
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idleAnim"))
        {
            trotting = false;
        }

        // start trotting if player gives input
        if (moveX != 0 && !trotting)
        {
            anim.SetTrigger("trot");
            trotting = true;
        }
        
        // flip sprite depending on direction of input
        if ((moveX < 0 && facingRight) || (moveX > 0 && !facingRight))
        {
            Flip();
        }

        // apply velocity, lerp between current and target velocity for smooth acceleration
        targetVelocity = Mathf.Lerp(rb.velocity.x, moveX * speed, 0.05f);
        rb.velocity = new Vector3(targetVelocity, rb.velocity.y, 0);

        CalculateSpeedMultiplier();
    }

    // flips sprite when player changes movement direction
    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
    }

    // stops trotting on specific frames if player has released input
    public void StopTrot(int frame)
    {
        if (moveX == 0)
        {
            switch (frame)
            {
                // stop here
                case 0 or 5 or 6 or 11:
                    anim.SetTrigger("trot");
                    trotting = false;
                    stepDirection = 1;
                    break;

                // step backwards here
                case 1 or 2 or 7 or 8:
                    stepDirection = -1;
                    break;

                // step forwards here
                case 3 or 4 or 9 or 10:
                    stepDirection = 1;
                    break;
            }
        }
        
        // step forwards again if still moving
        if (moveX != 0) {
            stepDirection = 1;
        }

        CalculateSpeedMultiplier();
    }

    void CalculateSpeedMultiplier()
    {
        // calculate trot "speed" multiplier
        float speedMultiplier = targetVelocity / speed;

        // disregard direction of movement
        speedMultiplier = Mathf.Abs(speedMultiplier);

        // clamp to minimum speed + scale magnitude of normal speed (makes for smoother transitions)
        speedMultiplier = Mathf.Max(0.7f, 1.1f * speedMultiplier);

        // multiply by "step direction" - determines whether animation plays forwards/backwards for smoother stopping
        speedMultiplier *= stepDirection;

        // send speed multiplier to animator parameter
        anim.SetFloat("speed", speedMultiplier);
    }
}
