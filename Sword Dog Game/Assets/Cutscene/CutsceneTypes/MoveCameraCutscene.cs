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
    public bool freeHudOnExit = false;
    public bool useMainCamera = true;

    public bool ignoreCameraBounds = false;

    public bool resetBarsOnExit = false;

    public Rigidbody2D cameraRb;

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
            target = CameraController.mainCam;
        if (useMainCamera)
            CameraController.main.externalControl = true;
        if (ignoreCameraBounds)
            CameraController.main.cldr.isTrigger = true;
        if(points.Count > 0)
            changingToNewTransform();
        if (target != null)
            cameraRb = target.GetComponent<Rigidbody2D>();
        
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (curPoint >= points.Count)
        {
            finishedSegment();
            return;
        }

        //If the distance between the point and the camera is <= .01f, OR if the point was overshot
        //AND the zoom is correct
        if (((Vector2.Distance(points[curPoint].targPos, target.transform.position) <= .01f && !points[curPoint].relPosition)
            || Vector2.Distance(points[curPoint].targPos + (Vector2)transform.position, target.transform.position) <= .01f && points[curPoint].relPosition)
            && Mathf.Abs(target.orthographicSize - points[curPoint].zoom) < .01f)
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

        if (points[curPoint].hideUI)
            CanvasManager.HideHUD(points[curPoint].instantUI);
        else
            CanvasManager.ShowHUD(points[curPoint].instantUI);
    }

    public void LinearMovement(CameraTransform transform)
    {
        LinearZoomChange(transform);
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
        if (transform.relPosition)
        {
            if (Vector2.Distance(target.transform.position, transform.targPos + (Vector2)this.transform.position) > .05f)
                target.transform.position += (Vector3)((transform.targPos + (Vector2)this.transform.position) - (Vector2)(target.transform.position)).normalized * Time.deltaTime * transform.speed;
            else
                target.transform.position = transform.targPos + (Vector2)this.transform.position;
        }
        else
        {
            if (Vector2.Distance(target.transform.position, transform.targPos) > .05f)
                target.transform.position += (Vector3)(transform.targPos - (Vector2)target.transform.position).normalized * Time.deltaTime * transform.speed;
            else
                target.transform.position = transform.targPos;
        }
        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
    }

    public void ExponentialMovement(CameraTransform transform)
    {
        ExponentialZoomChange(transform);
        ExponentialPositionChange(transform);
    }

    public void ExponentialZoomChange(CameraTransform transform)
    {
        target.orthographicSize -= (target.orthographicSize - transform.zoom) * Time.deltaTime * transform.speed;
    }

    public void ExponentialPositionChange(CameraTransform transform)
    {
        if(cameraRb != null && false)
        {
            if (transform.relPosition)
            {
                if (((target.transform.position - (Vector3)((Vector2)target.transform.position - (transform.targPos + (Vector2)this.transform.position)) * Time.deltaTime * transform.speed) - target.transform.position).magnitude > transform.minSpeed)
                    cameraRb.MovePosition(target.transform.position - (Vector3)((Vector2)target.transform.position - (transform.targPos + (Vector2)this.transform.position)) * Time.deltaTime * transform.speed);
                else
                    LinearMovement(transform);
            }
            else
            {
                if (((target.transform.position - (Vector3)((Vector2)target.transform.position - transform.targPos) * Time.deltaTime * transform.speed) - target.transform.position).magnitude > transform.minSpeed)
                    cameraRb.MovePosition(target.transform.position - (Vector3)((Vector2)target.transform.position - transform.targPos) * Time.deltaTime * transform.speed);
                else
                    LinearMovement(transform);
            }

            return;
        }
        if (transform.relPosition)
        {
            if ((((Vector3)((Vector2)target.transform.position - (transform.targPos + (Vector2)this.transform.position)) * Time.deltaTime * transform.speed)).magnitude > transform.minSpeed * Time.deltaTime)
                target.transform.position -= (Vector3)((Vector2)target.transform.position - (transform.targPos + (Vector2)this.transform.position)) * Time.deltaTime * transform.speed;
            else
                target.transform.position -= (Vector3)((Vector2)target.transform.position - (transform.targPos + (Vector2)this.transform.position)).normalized * Time.deltaTime * transform.speed;
        }
        else
        {
            if ((((Vector3)((Vector2)target.transform.position - transform.targPos) * Time.deltaTime * transform.speed)).magnitude > transform.minSpeed * Time.deltaTime)
            {
                target.transform.position -= (Vector3)((Vector2)target.transform.position - transform.targPos) * Time.deltaTime * transform.speed;
            }
            else
            {
                if (Vector2.Distance((((Vector3)((Vector2)target.transform.position - transform.targPos)).normalized * transform.minSpeed * Time.deltaTime), transform.targPos) > Time.deltaTime)
                    target.transform.position -= ((Vector3)((Vector2)target.transform.position - transform.targPos)).normalized * transform.minSpeed * Time.deltaTime;
                else
                    target.transform.position = transform.targPos;
            }
        }
    }

    public override void finishedSegment()
    {
        if (points.Count > 0)
        {
            if(points[^1].relPosition)
                target.transform.position = new Vector3(points[^1].targPos.x + transform.position.x, points[^1].targPos.y + transform.position.y, target.transform.position.z);
            else
                target.transform.position = new Vector3(points[^1].targPos.x, points[^1].targPos.y, target.transform.position.z);
        }
        if (useMainCamera && freeCameraOnExit)
        {
            CameraController.main.externalControl = false;
            if(freeHudOnExit)
                CanvasManager.ShowHUD(); // TODO this is kind of a hack fix - perhaps there's a better way to integrate things with the hideUI flag

            if (ignoreCameraBounds)
                CameraController.main.cldr.isTrigger = false;
        }
        
        if (CinematicBars.current != null  && resetBarsOnExit)
            CinematicBars.current.Hide();

        base.finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        if (useMainCamera && freeCameraOnExit)
        {
            CameraController.main.externalControl = false;
            if(freeHudOnExit)
                CanvasManager.ShowHUD(); // TODO this is kind of a hack fix - perhaps there's a better way to integrate things with the hideUI flag
        }
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
        public bool hideUI;
        public bool instantUI;
        public bool relPosition;
        public float minSpeed;

        public Transform targTransform;
        public bool useTransformX, useTransformY;
        public Vector2 targPos
        {
            get
            {
                if(targTransform == null)
                    return point;
                else
                {
                    return (new Vector2(useTransformX ? targTransform.position.x : point.x, useTransformY ? targTransform.position.y : point.y));
                }
            }
        }

        public CameraTransform(float zoom, float speed, Vector2 targetPoint, MovementType transition, bool letterbox = false, bool hideUI = false, bool instantUI = false, bool relPosition = false)
        {
            this.zoom = zoom;
            this.speed = speed;
            point = targetPoint;
            this.letterbox = letterbox;
            this.transition = transition;
            this.hideUI = hideUI;
            this.instantUI = instantUI;
            this.relPosition = relPosition;
            minSpeed = 0;
            targTransform = null;
            useTransformX = false;
            useTransformY = false;
        }
    }
}
