using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTracker;
    public Vector3 offset = new Vector3(0, 2, -10);

    public float speed = 5; 

    public static Action sceneChange;
    public static Vector3 newPos;

    public Vector3 targetPoint
    {
        get
        {
            return targetTracker.position + offset;
        }
    }

    void Awake()
    {
        transform.position = targetPoint;
    }

    void Start()
    {
        sceneChange += Snap;
    }

    // Update is called once per frame
    void Update()
    {
        targetTracker = PlayerMovement.instance.transform;
        //transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    private void FixedUpdate()
    {
        transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    // private void SceneChange()
    // {
    //     Invoke("Snap", 0.1f);
    // }

    private void Snap()
    {
        Camera.main.transform.position = newPos;
    }
}
