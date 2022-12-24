using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToPointCutscene : CutsceneData
{
    public Vector2 targetPoint;
    private Vector2 startPoint;

    public float startTime;
    public float travelTime = 1;

    public override void startSegment()
    {
        base.startSegment();
        startPoint = transform.position;
        startTime = Time.time;
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if(Time.time - startTime > travelTime)
        {
            finishedSegment();
            return;
        }

        transform.position = new Vector3(Mathf.Lerp(startPoint.x, targetPoint.x, (Time.time - startTime) / travelTime), Mathf.Lerp(startPoint.y, targetPoint.y, (Time.time - startTime) / travelTime));

    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        transform.position = targetPoint;
    }
}
