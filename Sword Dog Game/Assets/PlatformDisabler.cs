using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisabler : MonoBehaviour
{
    public Collider2D cldr;

    // Start is called before the first frame update
    void Start()
    {
        cldr = GetComponent<Collider2D>();   
    }

    private void FixedUpdate()
    {
        if(InputReader.inputs.actions["Move"].ReadValue<Vector2>().y >= .9f)
        {
            Physics2D.IgnoreCollision(cldr, PlayerMovement.controller.cldr, true);
        }
        else
            Physics2D.IgnoreCollision(cldr, PlayerMovement.controller.cldr, false);
    }
}