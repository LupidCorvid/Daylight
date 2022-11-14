using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class TextEffect
{
    public int start = 0;
    //End of -1 means no end
    public int end = -1;

    public abstract void ApplyEffectToMesh(TMP_Text textMesh);
}
