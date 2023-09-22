using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTracker;
    public Vector3 offset = new Vector3(0, 0, -10);

    public float speed = 5;
    public float defaultZoom = 5;
    public static Vector3 newPos;

    public static CameraController main;

    public static Camera mainCam;
    private static bool overrideMovement = false;
    private static float overrideFor = 0.5f, maxDelay = 0.5f;
    private static Vector3 overrideTracker;

    Rigidbody2D rb;

    public Vector2 RbFollowVector;

    public Vector3 targetPoint
    {
        get
        {
            if (!overrideMovement && targetTracker != null)
                return targetTracker.position + offset;
            if (overrideTracker == null)
                return Vector2.zero;
            return overrideTracker + offset;
        }
    }

    public BoxCollider2D cldr;
    public Rigidbody2D followrb;

    public bool externalControl = false;

    public bool lockX;
    public bool lockY;
    public bool lockZoom;

    public Vector3 lockDetails;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        main = this;
        mainCam = GetComponent<Camera>();
        cldr = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovement.instance != null)
            targetTracker = PlayerMovement.instance.transform;

        else
            targetTracker = GameObject.FindGameObjectWithTag("Player").transform;
        followrb = targetTracker.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        //transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    private void FixedUpdate()
    {
        if (overrideMovement)
        {
            overrideFor += Time.fixedDeltaTime;
            if (overrideFor > maxDelay)
                overrideMovement = false;
        }

        if (!externalControl)
        {
            Vector3 finalTarg = targetPoint;
            if (followrb != null)
            {
                RbFollowVector += Vector2.right * Input.GetAxis("Horizontal") * Time.deltaTime * 2;
                //RbFollowVector = Vector2.Lerp(RbFollowVector, Vector2.right * Input.GetAxisRaw("Horizontal") * 2, Time.deltaTime);

                //float magCap = Input.GetAxis("Horizontal"); //Link to slight inputs
                float magCap = 1;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    magCap *= 1.25f; //Increase view range when sprinting

                }
                
                RbFollowVector = new Vector2(Mathf.Clamp(RbFollowVector.x, -magCap, magCap), Mathf.Clamp(RbFollowVector.y, -magCap, magCap));
                finalTarg += (Vector3)RbFollowVector * 2;



                //Velocity based
                //if(Mathf.Abs(followrb.velocity.x) > .75f)
                //    RbFollowVector = Vector2.Lerp(new Vector3(followrb.velocity.x, 0, 0), RbFollowVector, .005f) * .45f;
                //finalTarg += (Vector3)RbFollowVector;
            }

            float targZoom = defaultZoom;

            if (lockX)
                finalTarg.x = lockDetails.x;
            if (lockY)
                finalTarg.y = lockDetails.y;
            if (lockZoom)
                targZoom = lockDetails.z;

            transform.position += (finalTarg - transform.position) * Time.deltaTime * speed;
            if (Camera.main.orthographicSize != targZoom)
            {
                Camera.main.orthographicSize -= (Camera.main.orthographicSize - targZoom) * Time.deltaTime;
                if (Mathf.Abs(Camera.main.orthographicSize - targZoom) < .01f)
                    Camera.main.orthographicSize = targZoom;
            }
        }

        rb.velocity = Vector2.zero;

        cldr.size = new Vector3(18, 10) * mainCam.orthographicSize / 5;
        
        
    }

    public static void OverrideMovement(Transform player, float duration = 0.5f)
    {
        duration = Mathf.Clamp(duration, 0, maxDelay);
        overrideMovement = true;
        overrideFor = maxDelay - duration;
        overrideTracker = player.position;
    }
}
