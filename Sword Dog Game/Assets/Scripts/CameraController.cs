using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTracker;
    public Vector3 offset = new Vector3(0, 2, -10);

    public float speed = 5; 

    public Vector3 targetPoint
    {
        get
        {
            return targetTracker.position + offset;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    private void FixedUpdate()
    {
        transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }
}
