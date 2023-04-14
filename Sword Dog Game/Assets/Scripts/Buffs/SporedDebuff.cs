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
        IndicatorGameObj = TempObjectsHolder.main.sporedDebuffPrefab;
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
            visuals.updateFillAmount((Time.time - (lastActivation + (Time.time - startTime))) / activationInterval);
    }

    public override void checkForCycleRun()
    {
        if (!active)
            return;
        if ((lastActivation + (Time.time - startTime)) + activationInterval <= Time.time)
        {
            lastActivation = Time.time;
            Execute();
        }
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
        affectedEntity.TakeDamage(1);
    }
}
