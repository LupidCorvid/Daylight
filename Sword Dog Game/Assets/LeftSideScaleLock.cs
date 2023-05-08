using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LeftSideScaleLock : MonoBehaviour
{
    public RectTransform rect;

    public Vector2 rectLeftSidePos = new Vector2();

    // Update is called once per frame
    void Update()
    {
        rect.transform.localPosition = rectLeftSidePos + (rect.sizeDelta / 2 * rect.localScale);
    }
}
