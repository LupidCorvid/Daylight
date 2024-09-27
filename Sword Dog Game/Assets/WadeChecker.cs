using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadeChecker : MonoBehaviour
{
    public int waterDepth = 0;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<WaterFX>() != null)
            waterDepth++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<WaterFX>() != null)
            waterDepth--;
    }
}
