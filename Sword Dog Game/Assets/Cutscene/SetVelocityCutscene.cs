using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVelocityCutscene : CutsceneData
{
    public List<VelocityRigidbodyPair> targets = new List<VelocityRigidbodyPair>();
    public bool AlsoSetOnLateUpdate = false;
    bool needsSet = false;
    public bool setX, setY;
    public float doForTime = 0;
    float startTime = 0;

    public override void startSegment()
    {
        base.startSegment();
        SetVelocities();
        if (AlsoSetOnLateUpdate)
            needsSet = true;
        startTime = Time.time;
        finishedSegment();
    }

    private void FixedUpdate()
    {
        if(Time.time < startTime + doForTime)
        {
            SetVelocities();
        }
    }

    private void LateUpdate()
    {
        if(AlsoSetOnLateUpdate && needsSet)
        {
            SetVelocities();
            needsSet = false;
        }

    }

    public void SetVelocities()
    {
        foreach (VelocityRigidbodyPair pair in targets)
        {
            pair.rb.velocity = new Vector2(setX ? pair.vel.x : pair.rb.velocity.x, setY ? pair.vel.y : pair.rb.velocity.y);
            Debug.Log("setting vel");
        }
    }

    [System.Serializable]
    public struct VelocityRigidbodyPair
    {
        public Rigidbody2D rb;
        public Vector2 vel;

        public VelocityRigidbodyPair(Rigidbody2D inRb, Vector2 velocity)
        {
            rb = inRb;
            vel = velocity;
        }
    }

}
