using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxAnimationController : MonoBehaviour
{
    public void finishOpen()
    {
        Debug.Log("Opened");
        DialogController.main.finishOpen();
    }

    public void finishClose()
    {
        Debug.Log("Closed");
        DialogController.main.finishClose();
    }
}
