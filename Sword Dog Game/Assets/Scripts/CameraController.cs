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

    public bool externalControl = false;

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
            transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
            if (Camera.main.orthographicSize != defaultZoom)
            {
                Camera.main.orthographicSize -= (Camera.main.orthographicSize - defaultZoom) * Time.deltaTime;
                if (Mathf.Abs(Camera.main.orthographicSize - defaultZoom) < .01f)
                    Camera.main.orthographicSize = defaultZoom;
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
