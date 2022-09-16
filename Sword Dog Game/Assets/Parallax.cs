using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Copy pasted from the other branch
public class Parallax : MonoBehaviour
{
    public float distance = 0;
    public Camera cam;
    public Vector3 startPos;
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
        transform.position = new Vector3(startPos.x + ((startPos.x - cam.transform.position.x) * distance), startPos.y + ((startPos.y - cam.transform.position.y) * distance), transform.position.z);
    }
}
