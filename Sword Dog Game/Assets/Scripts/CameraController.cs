using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTracker;
    public Vector3 offset = new Vector3(0, 2, -10);

    public float speed = 5;
    public float defaultZoom = 5;
    public static Vector3 newPos;

    public static CameraController main;

    public static Camera mainCam;
    private static bool canMove = true;
    private static float cantMoveFor = 0.1f, maxDelay = 0.1f;

    Rigidbody2D rb;

    public Vector3 targetPoint
    {
        get
        {
            return targetTracker.position + offset;
        }
    }

    public Collider2D cldr;

    public bool externalControl = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        main = this;
        mainCam = GetComponent<Camera>();
        cldr = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        targetTracker = PlayerMovement.instance.transform;
        rb.velocity = Vector2.zero;
        //transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            cantMoveFor += Time.fixedDeltaTime;
            if (cantMoveFor < maxDelay)
                return;
            canMove = true;
        }

        if (!externalControl)
        {
            transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
            if (Camera.main.orthographicSize != defaultZoom)
            {
                Camera.main.orthographicSize -= (Camera.main.orthographicSize - defaultZoom) * Time.deltaTime;
                if (Mathf.Abs(Camera.main.orthographicSize - defaultZoom) < .05f)
                    Camera.main.orthographicSize = defaultZoom;
            }
        }
    }

    public static void DisableMovement()
    {
        canMove = false;
        cantMoveFor = 0f;
    }
}
