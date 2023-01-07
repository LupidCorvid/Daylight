using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveCameraCutscene : CutsceneData
{

    public List<CameraTransform> points = new List<CameraTransform>();
    int curPoint = 0;

    public Camera target;

    public bool freeCameraOnExit = true;

    public bool useMainCamera = true;

    public enum MovementType
    {
        Linear,
        Exponential,
        CappedExponential
    }

    public override void startSegment()
    {
        base.startSegment();
        curPoint = 0;
        if(useMainCamera)
            target = Camera.main;
        if (useMainCamera)
            CameraController.main.externalControl = true;
        if(points.Count > 0)
            changingToNewTransform();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (curPoint >= points.Count)
        {
            finishedSegment();
            return;
        }

        if (Vector2.Distance(points[curPoint].point, target.transform.position) <= .05f && Mathf.Abs(target.orthographicSize - points[curPoint].zoom) < .05f)
        {
            curPoint++;
            if (curPoint < points.Count)
                changingToNewTransform();
            return;
        }



        switch (points[curPoint].transition)
        {
            case MovementType.Linear:
                LinearMovement(points[curPoint]);
                break;
            case MovementType.Exponential:
                ExponentialMovement(points[curPoint]);
                break;

        }

    }

    public void changingToNewTransform()
    {
        if (points[curPoint].letterbox && !CinematicBars.current.beingAdded)
            CinematicBars.current.Show();
        else if (!points[curPoint].letterbox && CinematicBars.current.beingAdded)
            CinematicBars.current.Hide();
    }

    public void LinearMovement(CameraTransform transform)
    {
        LinearZoomChange(transform);
        if (Vector2.Distance(points[curPoint].point, target.transform.position) > .05f)
            LinearPositionChange(transform);
    }

    public void LinearZoomChange(CameraTransform transform)
    {
        if (target.orthographicSize - points[curPoint].zoom > 0)
            target.orthographicSize -= 1 * transform.speed * Time.deltaTime;
        else
            target.orthographicSize += 1 * transform.speed * Time.deltaTime;
    }

    public void LinearPositionChange(CameraTransform transform)
    {
        if (Vector2.Distance(target.transform.position, (Vector3)transform.point) > .05f)
            target.transform.position += ((Vector3)transform.point - target.transform.position).normalized * Time.deltaTime * transform.speed;
    }

    public void ExponentialMovement(CameraTransform transform)
    {
        ExponentialZoomChange(transform);
        ExponentialPoisitionChange(transform);
    }

    public void ExponentialZoomChange(CameraTransform transform)
    {
        target.orthographicSize -= (target.orthographicSize - transform.zoom) * Time.deltaTime;
    }

    public void ExponentialPoisitionChange(CameraTransform transform)
    {
        target.transform.position -= (Vector3)((Vector2)target.transform.position - transform.point) * Time.deltaTime;
    }

    public override void finishedSegment()
    {
        if (points.Count > 0)
            target.transform.position = new Vector3(points[^1].point.x, points[^1].point.y, target.transform.position.z);
        if (useMainCamera && freeCameraOnExit)
            CameraController.main.externalControl = false;
        if (CinematicBars.current.beingAdded)
            CinematicBars.current.Hide();
        base.finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        if (useMainCamera && freeCameraOnExit)
            CameraController.main.externalControl = false;
        if (CinematicBars.current.beingAdded)
            CinematicBars.current.Hide();
    }

    [Serializable]
    public struct CameraTransform
    {
        public float zoom;
        public float speed;
        public Vector2 point;
        public bool letterbox;
        public MovementType transition;


        public CameraTransform(float zoom, float speed, Vector2 targetPoint, MovementType transition, bool letterbox = false)
        {
            this.zoom = zoom;
            this.speed = speed;
            point = targetPoint;
            this.letterbox = letterbox;
            this.transition = transition;
        }
    }
}
