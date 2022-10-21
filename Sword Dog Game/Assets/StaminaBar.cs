using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    public float fillAmount = 1;
    private float defaultWidth;
    private float basePosition;

    private float originalWidth;

    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        defaultWidth = rect.localScale.x;
        basePosition = rect.position.x;
        basePosition = rect.rect.x;
        basePosition = rect.position.x - rect.rect.width/2;
        Debug.Log(rect.position.x);
        Debug.Log(basePosition);
        //originalWidth = rect.rect.width;
    }
    void Update()
    {
        setFillAmount(fillAmount);
    }
    public void setFillAmount(float amount)
    {
        rect.localScale = new Vector3(defaultWidth * amount, rect.localScale.y, rect.localScale.z);
        rect.position = new Vector3(basePosition + (rect.rect.width * rect.localScale.x) /1, rect.position.y, rect.position.z);
        //(basePosition - (rect.localScale.x))
    }
}
