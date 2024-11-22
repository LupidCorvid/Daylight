using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeItem : MonoBehaviour
{
    Color logAlpha;
    bool exitedLog;

    // Start is called before the first frame update
    void Start()
    {
        logAlpha = gameObject.GetComponent<SpriteRenderer>().color;
        logAlpha.a = 1;
        exitedLog = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().color = logAlpha;

        if (exitedLog)
        {
            if(logAlpha.a < 1)
            {
                logAlpha.a += .02f;
            }
            else
            {
                logAlpha.a = 1;
                exitedLog = false;
            }
        }
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
           if(logAlpha.a > 0)
           {
                logAlpha.a -= 0.02f;
           }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            exitedLog = true;
        }
    }
}
