using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffs
{
    public SporedDebuff spored;
    public MoveSpeedBuff moveSpeedBuff;

    public SaveBuffs buffsSave;

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
        buffsSave = new SaveBuffs();
        spored.UpdateSave(this);
        moveSpeedBuff.UpdateSave(this);
        GameSaver.currData.buffs = buffsSave;
    }

    public void loadBuffs(GameSaver.SaveData data)
    {
        if (data?.buffs == null)
            return;
        moveSpeedBuff.LoadSave(data.buffs.moveSpeed);
        spored.LoadSave(data.buffs.spored);
    }

    public void Update()
    {
        spored?.Update();
        moveSpeedBuff?.Update();
    }

    [System.Serializable]
    public class SaveBuffs
    {
        public MoveSpeedBuff.MoveSpeedBuffSave moveSpeed;
        public SporedDebuff.SporedDebuffSave spored;
    }
}
