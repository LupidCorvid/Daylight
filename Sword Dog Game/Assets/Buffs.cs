using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buffs
{
    public SporedDebuff spored;
    public MoveSpeedBuff moveSpeedBuff;

    public Buffs(Entity holder)
    {
        initializeBuffs(holder);
        if(holder as Player != null)
        {
            GameSaver.StartingSave += saveBuffs;
        }
    }

    public void initializeBuffs(Entity holder)
    {
        spored = new SporedDebuff(holder);
        moveSpeedBuff = new MoveSpeedBuff(holder);
    }

    public void saveBuffs()
    {
        GameSaver.currData.buffs = this;
    }

    public void Update()
    {
        spored?.Update();
    }
}
