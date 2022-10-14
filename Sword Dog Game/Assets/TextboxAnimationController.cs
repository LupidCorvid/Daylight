using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxAnimationController : MonoBehaviour
{
    public void finishOpen()
    {
        DialogController.main.finishOpen();
    }

    public void finishClose()
    {
        DialogController.main.finishClose();
    }
}
