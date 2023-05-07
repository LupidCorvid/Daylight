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

    public override void Inflict(float strength)
    {
        if (strength > intensity)
            intensity = strength;

        base.Inflict();
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
        affectedEntity.removeBufffDisplay(buffID);
    }

    public override void UpdateSave(Buffs manager)
    {
        base.UpdateSave(manager);
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
