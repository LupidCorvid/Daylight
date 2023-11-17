using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporedDebuff : Buff
{
    public SporeIndicator visuals;
    public GameObject IndicatorGameObj;

    public SporedDebuff(Entity entity)
    {
        duration = 1f;
        activationInterval = 3;
        affectedEntity = entity;

    }


    public override void Inflict()
    {
        if(Time.time - startTime > .05f)    
            lastActivation += (Time.time - startTime) * 2;

        IndicatorGameObj ??= TempObjectsHolder.main.sporedDebuffPrefab;
        base.Inflict();
    }

    public override void Update()
    {
        base.Update();
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if(active && visuals != null)
            visuals.updateFillAmount((Time.time - (lastActivation + 2 * (Time.time - startTime))) / activationInterval);
        //Tt - lA + 2(Tt sT) = aI
    }

    public override void checkForCycleRun()
    {
        if (!active)
            return;
        //if ((lastActivation + 2 * (Time.time - startTime)) + activationInterval <= Time.time)
        if(Time.time - (lastActivation + 2 * (Time.time - startTime)) >= activationInterval)
        {
            lastActivation = Time.time;
            Execute();
        }
    }

    public override void checkDuration()
    {
        if (!active)
            return;
        if (Time.time - (lastActivation + 2 * (Time.time - startTime)) < 0 && startTime + duration <= Time.time)
            Cure();
    }

    public override void enableVisuals()
    {
        base.enableVisuals();
        GameObject addedGameObject = Entity.Instantiate(IndicatorGameObj, CanvasManager.instance.transform);
        visuals = addedGameObject.GetComponent<SporeIndicator>();
        visuals.target = affectedEntity.transform;
    }

    public override void disableVisuals()
    {
        base.disableVisuals();
        Entity.Destroy(visuals.gameObject);
    }

    public override void Execute()
    {
        base.Execute();
        affectedEntity.TakeDamage(1, null);
    }

    public override void UpdateSave(Buffs manager)
    {
        base.UpdateSave(manager);
        if (affectedEntity == Player.controller.entityBase)
            manager.buffsSave.spored = new SporedDebuffSave(this);
    }

    [System.Serializable]
    public class SporedDebuffSave : SavedBuff 
    {
        public SporedDebuffSave(Buff buff) : base(buff)
        {

        }
    }
}
