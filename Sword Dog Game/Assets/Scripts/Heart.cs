using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    // 0 = empty, 1 = half, 2 = full
    public Sprite[] sprites;

    public void SetSprite(int type)
    {
        GetComponent<Image>().sprite = sprites[Mathf.Clamp(type, 0, 2)];
    }

}
