using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SporeIndicator : MonoBehaviour
{
    public Image background;
    public Image fillImage;
    public Transform target;


    public void updateFillAmount(float newAmount)
    {
        fillImage.fillAmount = newAmount;
    }

    public void Update()
    {
        if(target != null)
            transform.position = Camera.main.WorldToScreenPoint((Vector2)target.position + Vector2.up * 2);
    }
}
