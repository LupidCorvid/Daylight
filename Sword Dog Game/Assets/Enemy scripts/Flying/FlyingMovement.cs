using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMovement : BaseMovement
{
    public void MoveDirection(Vector2 targetVelocity)
    {
        cldr.sharedMaterial = slippery;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(targetVelocity.x, targetVelocity.y), ref velocity, movementSmoothing);
    }
}
