using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam
{
    [System.Flags]
    public enum Team
    {
        Player = 0b_01,
        Enemy = 0b_10
    }

    public Team allies
    {
        get;
        set;
    }
    public Team enemies
    {
        get;
        set;
    }

    public virtual bool GetIfEnemies(ITeam otherEntity)
    {
        return ((enemies & otherEntity.allies) > 0 && otherEntity != this);
    }
}
