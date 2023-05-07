using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSpeedBuff : Buff
{

    public MoveSpeedBuff(Entity entity)
    {
        affectedEntity = entity;
    }

    public override void Inflict()
    {
        Inflict(1);
    }

    public override void Inflict(float strength)
    {
        
        if (strength > intensity)
        {
            if (active)
                affectedEntity.moveSpeed.multiplier -= strength;
            intensity = strength;
            affectedEntity.moveSpeed.multiplier += intensity;
        } else if (!active)
            affectedEntity.moveSpeed.multiplier += intensity;

        base.Inflict();
    }

    public override void Inflict(float duration, float intensity)
    {

        Inflict(intensity);

        if (startTime + this.duration <= Time.time + duration)
        {
            startTime = Time.time;
            this.duration = duration;
        }
    }

    public override void Cure()
    {
        base.Cure();
        affectedEntity.moveSpeed.multiplier -= intensity;
    }

    public override void enableVisuals()
    {
        base.enableVisuals();
        GameObject buffIcon = affectedEntity.addBuffDisplay(buffID, TempObjectsHolder.main.buffIconDisplay);
        if(buffIcon != null)
            buffIcon.GetComponent<Image>().sprite = TempObjectsHolder.main.FindSprite("Buffs.SpeedUp");
    }

    public override void disableVisuals()
    {
        base.disableVisuals();
        affectedEntity.removeBuffDisplay(buffID);
    }

    public override void LoadSave(SavedBuff save)
    {
        Inflict(save.remainingDuration, save.intensity);
    }

    public override void UpdateSave(Buffs manager)
    {
        base.UpdateSave(manager);
        if(affectedEntity == PlayerMovement.controller.entityBase)
            manager.buffsSave.moveSpeed = new MoveSpeedBuffSave(this);
    }

    [System.Serializable]
    public class MoveSpeedBuffSave : SavedBuff 
    {
        public MoveSpeedBuffSave(Buff buff) : base(buff)
        {

        }
    }



}
