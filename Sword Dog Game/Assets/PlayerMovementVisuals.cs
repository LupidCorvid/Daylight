using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementVisuals : MonoBehaviour
{
    public PlayerMovement baseObject;

    public float landTransition;

    public void stopTrot(int frame)
    {
        baseObject.StopTrot(frame);
    }

    private void FixedUpdate()
    {
        if(baseObject.isGrounded)
        {
            transform.rotation = Quaternion.Euler(0, 0, (Mathf.Lerp(baseObject.lastGroundedSlope, baseObject.slopeSideAngle, landTransition)));
        }
    }
}
