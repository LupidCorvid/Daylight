using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCollidersCutscene : CutsceneData
{
    public List<Collider2D> collidersToModify = new List<Collider2D>();
    public bool setEnable = true;

    public override void startSegment()
    {
        foreach(Collider2D collider in collidersToModify)
        {
            collider.enabled = setEnable;
        }

        finishedSegment();
    }
}
