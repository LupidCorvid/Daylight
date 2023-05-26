using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDebuff : Buff
{

    public StunDebuff(Entity entity)
    {
        affectedEntity = entity;
        entity.damaged += ((giver) => Cure());
    }

    public override void Inflict()
    {
        //if(!active)
        //{
        //    affectedEntity.moveSpeed.setZeroSources++;
        //    affectedEntity.attackSpeed.setZeroSources++;
        //}
        if (!active)
            affectedEntity.stunned = true;
        base.Inflict();
    }

    public override void Inflict(float duration, float intensity)
    {

        if (!active)
            affectedEntity.stunned = true;
        base.Inflict(duration, intensity);
    }

    public override void Cure()
    {
        affectedEntity.stunned = false;
        base.Cure();
    }

    public override void UpdateSave(Buffs manager)
    {
        base.UpdateSave(manager);
        if (affectedEntity == PlayerMovement.controller.entityBase)
            manager.buffsSave.stunned = new StunnedBuffSave(this);
    }
    [System.Serializable]
    public class StunnedBuffSave : SavedBuff
    {
        public StunnedBuffSave(Buff buff) : base(buff)
        {

        }
    }
}
