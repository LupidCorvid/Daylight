using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeyederEye : MonoBehaviour
{
    public Vector2 anchorPosition;
    public Speyeder holder;
    public float eyeRadius = .2f;

    public void Awake()
    {
        anchorPosition = transform.localPosition;
    }

    public void FixedUpdate()
    {
        if (holder.ai.target != null)
            LookAtPoint(holder.ai.target.position);
        else
        {
            Vector2 lookPoint = new Vector2(transform.position.x + (Mathf.PerlinNoise(Time.time/5f, 0) - 0.5f) * 40, transform.position.y - 10);
            LookAtPoint(lookPoint);
        }
    }

    public void LookAtPoint(Vector2 point)
    {
        Vector2 localSpacePoint = (point - ((anchorPosition * transform.localScale) + (Vector2)transform.position))/5f;
        if(localSpacePoint.magnitude > 1)
            transform.localPosition = (anchorPosition + (localSpacePoint.normalized) * eyeRadius) * transform.localScale;
        else
            transform.localPosition = (anchorPosition + (localSpacePoint) * eyeRadius) * transform.localScale;
    }
}
