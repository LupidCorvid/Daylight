using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRegion : MonoBehaviour
{
    public Vector3 addOffset;

    public bool lockX;
    public bool lockY;
    public bool lockZoom;

    public bool lockToPoint;
    public Vector2 lockPosition;

    public float zoomLock;

    public bool ignoreColliders = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player(Clone)")
            return;

        Debug.Log("hit " + collision.gameObject.name + "|" + CameraController.main.GetInstanceID());
        CameraController.main.offset += addOffset;
        CameraController.main.lockX = lockX;
        CameraController.main.lockY = lockY;
        CameraController.main.lockZoom = lockZoom;
        if (lockToPoint)
            CameraController.main.lockDetails = new Vector3(lockPosition.x, lockPosition.y, CameraController.mainCam.orthographicSize);
        else
            CameraController.main.lockDetails = new Vector3(CameraController.mainCam.transform.position.x, CameraController.mainCam.transform.position.y, CameraController.mainCam.orthographicSize);
        if (lockZoom)
            CameraController.main.lockDetails.z = zoomLock;
        if (ignoreColliders)
            CameraController.main.cldr.isTrigger = true;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player(Clone)")
            return;
        CameraController.main.offset -= addOffset;
        if(lockX)
            CameraController.main.lockX = false;
        if(lockY)
            CameraController.main.lockY = false;
        if(lockZoom)
            CameraController.main.lockZoom = false;

        if (ignoreColliders)
            CameraController.main.cldr.isTrigger = false;

    }
}
