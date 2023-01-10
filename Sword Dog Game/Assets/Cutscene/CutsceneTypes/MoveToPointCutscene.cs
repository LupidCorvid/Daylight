using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToPointCutscene : CutsceneData
{
    private Vector2 startPoint;

    public float startTime;

    public List<PositionTime> points = new List<PositionTime>();
    int curPoint = 0;

    public override void startSegment()
    {
        base.startSegment();
        startPoint = transform.position;
        startTime = Time.time;
        curPoint = 0;
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if(curPoint >= points.Count)
        {
            finishedSegment();
            return;
        }

        if(Time.time - startTime >= points[curPoint].travelTime)
        {       
            startPoint = points[curPoint].toPoint;
            curPoint++;
            startTime = Time.time;
            return;
        }

        transform.position = new Vector3(Mathf.Lerp(startPoint.x, points[curPoint].toPoint.x, (Time.time - startTime) / points[curPoint].travelTime), Mathf.Lerp(startPoint.y, points[curPoint].toPoint.y, (Time.time - startTime) / points[curPoint].travelTime));

    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        if(points.Count > 0)
            transform.position = points[^1].toPoint;
    }

    [Serializable]
    public struct PositionTime
    {
        public float travelTime;
        public Vector2 toPoint;

        public PositionTime(float time, Vector2 point)
        {
            travelTime = time;
            toPoint = point;
        }
    }
}
