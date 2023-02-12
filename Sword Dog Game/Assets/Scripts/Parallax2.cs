using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax2 : MonoBehaviour
{
    public Vector2 distance = new Vector2(0,0);
    public Camera cam;
    public Vector3 startPos;

    public bool changeOnZoom = true;

    // Start is called before the first frame update
    void Start()
    {
        cam ??= Camera.main;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (cam == null)
            cam = Camera.main;
        if(changeOnZoom)
            transform.position = new Vector3(startPos.x + ((startPos.x - cam.transform.position.x) * distance.x * 5f / cam.orthographicSize), startPos.y + ((startPos.y - cam.transform.position.y) * distance.y * 5f / cam.orthographicSize), transform.position.z);
        else
            transform.position = new Vector3(startPos.x + ((startPos.x - cam.transform.position.x) * distance.x ), startPos.y + ((startPos.y - cam.transform.position.y) * distance.y ), transform.position.z);
    }
}
