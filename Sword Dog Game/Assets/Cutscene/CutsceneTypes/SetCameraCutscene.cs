using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraCutscene : CutsceneData
{

    public Camera target;
    public Vector2 position;
    public float zoom;
    public bool useMainCamera = true;
    public bool ignoreCameraBounds = false;
    public Rigidbody2D cameraRb;


    public override void startSegment()
    {
        base.startSegment();
        if (useMainCamera)
            target = CameraController.mainCam;
        if (useMainCamera)
            CameraController.main.externalControl = true;
        if (ignoreCameraBounds)
            CameraController.main.cldr.isTrigger = true;
        if (target != null)
            cameraRb = target.GetComponent<Rigidbody2D>();

        target.transform.position = new Vector3(position.x, position.y, target.transform.position.z);
        target.orthographicSize = zoom;
        finishedSegment();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
