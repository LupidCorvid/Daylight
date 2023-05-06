using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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
        buffIcon.GetComponent<Image>().sprite = TempObjectsHolder.main.sprites.movementBuff;
    }

    public override void disableVisuals()
    {
        base.disableVisuals();
        affectedEntity.removeBufffDisplay(buffID);
    }
    

}
