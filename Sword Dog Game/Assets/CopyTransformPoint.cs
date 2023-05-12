using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformPoint : MonoBehaviour
{

    public Transform target;
    public bool copyScale = true;
    public bool absScale = true;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    private void FixedUpdate()
    {
        if (target == null)
            return;
        transform.position = target.position;
        transform.rotation = target.rotation;
        if(copyScale)
        {
            if (absScale)
                transform.localScale = new Vector3(Mathf.Abs(target.lossyScale.x), Mathf.Abs(target.lossyScale.y), Mathf.Abs(target.lossyScale.z));
            else
                transform.localScale = new Vector3(target.lossyScale.x, target.lossyScale.y, target.lossyScale.z);
        }
    }
}
