using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisabler : MonoBehaviour
{
    public Collider2D cldr;
    JumpThrough jumpThrough;

    private float startTime = -5;
    private float holdLength = .5f;

    // Start is called before the first frame update
    void Start()
    {
        cldr = GetComponent<Collider2D>();
        jumpThrough = GetComponent<JumpThrough>();
    }

    private void FixedUpdate()
    {
        if (Player.controller != null)
        {
            if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().y >= .9f && cldr.IsTouching(Player.controller.cldr))
                startTime = Time.time;

            if (startTime + holdLength > Time.time)
                Physics2D.IgnoreCollision(cldr, Player.controller.cldr, true);
            else if (jumpThrough != null && jumpThrough.player.bottom.y < transform.position.y - jumpThrough.height)
                Physics2D.IgnoreCollision(cldr, Player.controller.cldr, false);
            
        }
    }
}
