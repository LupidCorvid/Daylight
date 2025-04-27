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

    private float animMinTime = 0.05f;
    private float animStartTime = 0;

    public Entity FreezeAI;

    public override void startSegment()
    {
        if (targetByName)
            target = GameObject.Find(targetName).GetComponent<Animator>();
        base.startSegment();
        speedOnEnter = target.speed;
        currState = 0;
        if (stateNames.Count > 0)
        {
            target.speed = stateNames[0].speed;
            target.Play(stateNames[0].name);
            AnimatorClipInfo[] animatorInfo = target.GetCurrentAnimatorClipInfo(0);
            Debug.Log("Started playing: " + animatorInfo[0].clip.name + "When trying to play " + stateNames[0].name);
            animStartTime = Time.time;
        }
        else
            finishedSegment();

        if (FreezeAI)
        {
            FreezeAI.freezeAI = true;
            if (FreezeAI as EnemyBase != null)
                ((EnemyBase)FreezeAI)?.movement?.SetSlippery(false);
        }
    }

    public override void cycleExecution()
    {
        base.cycleExecution();

        

        //AnimatorClipInfo[] animatorInfo = target.GetCurrentAnimatorClipInfo(0);
        //Debug.Log(animatorInfo[0].clip.name);

        if (Time.time < animStartTime + animMinTime)
            return;

        if (!target.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currState].name)
            || (target.GetCurrentAnimatorStateInfo(0).normalizedTime < target.GetCurrentAnimatorStateInfo(0).length
            && !target.GetCurrentAnimatorStateInfo(0).loop
            && target.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currState].name)))
        //if (animatorInfo[0].clip.name != stateNames[currState].name
        //    || (target.GetCurrentAnimatorStateInfo(0).normalizedTime < target.GetCurrentAnimatorStateInfo(0).length
        //    && !target.GetCurrentAnimatorStateInfo(0).loop
        //    && animatorInfo[0].clip.name == stateNames[currState].name))
        {
            currState++;
            if (currState < stateNames.Count)
            {
                target.Play(stateNames[currState].name);
                animStartTime = Time.time;
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
        if (FreezeAI)
            FreezeAI.freezeAI = false;
    }

    [System.Serializable]
    public class animState
    {
        public string name = "";
        public float speed = 1;
        public bool loop = false;
    }


}
