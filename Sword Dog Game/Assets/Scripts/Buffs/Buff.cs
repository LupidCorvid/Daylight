using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public Entity affectedEntity;
    public int stacks;

    public float duration = 1;
    public float startTime;

    public float intensity;

    public bool immune;

    public float activationInterval = 1;
    public float lastActivation;

    public bool active;

    public static int totalBuffs;

    public int buffID;

    public Buff()
    {
        buffID = totalBuffs;
        totalBuffs++;
    }

    public virtual void Inflict()
    {
        if (!active)
        {
            enableVisuals();
            lastActivation = Time.time;
        }

        active = true;

        if(startTime + duration <= Time.time + duration)
        {
            startTime = Time.time;
        }
    }

    public virtual void Inflict(float strength)
    {
        Inflict();
    }


    public virtual void Inflict(float duration, float intensity)
    {
        if (!active)
            enableVisuals();

        active = true;

        if (startTime + this.duration <= Time.time + duration)
        {
            startTime = Time.time;
            this.duration = duration;
        }
        this.intensity = this.intensity > intensity ? this.intensity : intensity;
    }

    public virtual void Cure()
    {
        disableVisuals();
        active = false;
    }

    public virtual void Execute()
    {

    }

    public virtual void enableVisuals()
    {

    }

    public virtual void disableVisuals()
    {

    }

    public virtual void updateVisuals()
    {

    }

    public virtual void checkDuration()
    {
        if (!active)
            return;
        if(startTime + duration <= Time.time)
        {
            Cure();
        }
    }

    public virtual void checkForCycleRun()
    {
        if (!active)
            return;
        if(lastActivation + activationInterval <= Time.time)
        {
            lastActivation = Time.time;
            Execute();
        }
    }

    public virtual void Update()
    {
        if (!active)
            return;
        checkForCycleRun();
        checkDuration();
    }

    public float GetRemainingTime()
    {
        return (duration - (Time.time - startTime));
    }

    public virtual void UpdateSave(Buffs manager)
    {

    }

    public virtual void LoadSave(SavedBuff save)
    {
        active = save.active;
        if (active)
        {
            Inflict();
            //enableVisuals();
        }

        //Debug.Log("Affected entity " + affectedEntity);
        intensity = save.intensity;
        //duration = save.remainingDuration;
        duration = save.totalDuration;
        startTime = save.remainingDuration - save.totalDuration;
        //startTime = Time.time;
        
    }

    [System.Serializable]
    public class SavedBuff
    {
        public float totalDuration;
        public float remainingDuration;
        public float intensity;
        public bool active;

        public SavedBuff()
        {

        }

        public SavedBuff(Buff buff)
        {
            remainingDuration = buff.GetRemainingTime();
            totalDuration = buff.duration;
            active = buff.active;
            intensity = buff.intensity;
        }
    }


}
