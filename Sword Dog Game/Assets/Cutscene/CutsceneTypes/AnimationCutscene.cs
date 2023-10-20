using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCutscene : CutsceneData
{
    public Animator target;
    public bool targetByName;
    public string targetName;

    public List<animState> stateNames = new List<animState>();

    public int currState = 0;

    public bool stopOnFinish = false;
    public bool restoreSpeedOnFinish = false;
    private float speedOnEnter;

    public override void startSegment()
    {
        if (targetByName)
            target = GameObject.Find(targetName).GetComponent<Animator>();
        base.startSegment();
        speedOnEnter = target.speed;
        if (stateNames.Count > 0)
        {
            target.speed = stateNames[0].speed;
            target.Play(stateNames[0].name);
        }
        else
            finishedSegment();
    }

    public override void cycleExecution()
    {
        base.cycleExecution();
        if (!target.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currState].name) 
            || (target.GetCurrentAnimatorStateInfo(0).normalizedTime < target.GetCurrentAnimatorStateInfo(0).length 
            && !target.GetCurrentAnimatorStateInfo(0).loop 
            && target.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currState].name)))
        {
            currState++;
            if (currState < stateNames.Count)
            {
                target.Play(stateNames[currState].name);
                target.speed = stateNames[currState].speed;
            }
            else
                finishedSegment();
        }
        else if (target.speed == 0)
            finishedSegment();
    }

    public override void finishedSegment()
    {
        base.finishedSegment();
        if (stopOnFinish)
            target.StopPlayback();
        if (restoreSpeedOnFinish)
            target.speed = speedOnEnter;
    }

    [System.Serializable]
    public class animState
    {
        public string name;
        public float speed;
    }


}
