using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSpeedBuff : Buff
{

    Image visualsImage;

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
                affectedEntity.moveSpeed.baseMultiplier -= strength;
            intensity = strength;
            affectedEntity.moveSpeed.baseMultiplier += intensity;
        } else if (!active)
            affectedEntity.moveSpeed.baseMultiplier += intensity;

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
        affectedEntity.moveSpeed.baseMultiplier -= intensity;
    }

    public override void enableVisuals()
    {
        base.enableVisuals();
        GameObject buffIcon = affectedEntity.addBuffDisplay(buffID, TempObjectsHolder.main.buffIconDisplay);
        if (buffIcon != null)
        {
            visualsImage = buffIcon.GetComponent<Image>();
            visualsImage.sprite = TempObjectsHolder.main.FindSprite("Buffs.SpeedUp");
            visualsImage.fillAmount= (Time.time - startTime)/duration;
            visualsImage.transform.GetChild(0).GetComponent<Image>().sprite = TempObjectsHolder.main.FindSprite("Buffs.SpeedUp");

        }
    }

    public override void Update()
    {
        base.Update();
        updateVisuals();
    }

    public override void updateVisuals()
    {
        if(visualsImage != null)
            visualsImage.fillAmount = GetRemainingTime() / duration;
    }

    public override void disableVisuals()
    {
        base.disableVisuals();
        affectedEntity.removeBuffDisplay(buffID);
    }

    public override void LoadSave(SavedBuff save)
    {
        //Inflict(save.remainingDuration, save.intensity);
        if (!active && save.active)
            enableVisuals();

        active = save.active;
        if (active)
        {
            intensity = save.intensity;

            Inflict(save.totalDuration, intensity);
    
            duration = save.totalDuration;
            startTime = Time.time + (save.remainingDuration - save.totalDuration);

        }
    }

    public override void UpdateSave(Buffs manager)
    {
        base.UpdateSave(manager);
        if(affectedEntity == Player.controller.entityBase)
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
