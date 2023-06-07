using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParryable
{
    public abstract bool canBeParried
    {
        get;
        set;
    }

    public void Parried(SwordFollow parriedBy);

    public void Blocked(SwordFollow parriedBy);
}
