using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBubbleCutscene : CutsceneData
{
    public GameObject bubblePrefab;
    public Vector2 position;

    public string dialog;

    GameObject addedBubble;

    public override void startSegment()
    {
        base.startSegment();
        //addedBubble = Instantiate(bubblePrefab, position, Quaternion.identity, TempObjectsHolder.asTransform);
        SpeakMiniBubble();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (addedBubble == null)
            finishedSegment();
    }

    public void SpeakMiniBubble()
    {
        GameObject addedObj = Instantiate(bubblePrefab, position, Quaternion.identity, TempObjectsHolder.asTransform);
        MiniBubbleController bubble = addedObj.GetComponent<MiniBubbleController>();
        bubble.setPosition = position;
        bubble.setSource(new DialogSource(dialog));

    }

}
