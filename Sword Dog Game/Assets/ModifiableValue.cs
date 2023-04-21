using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifiableValue
{
    public float baseValue = 1;

    public float multiplier = 1;

    public float additive = 0;

    public float additiveBase = 0;

    public float finalValue
    {
        get
        {
            return (baseValue + additiveBase) * multiplier + additive;
        }
    }

    public int finalIntValue
    {
        get
        {
            return (int)finalValue;
        }
    }

    public static implicit operator float(ModifiableValue other) => other.finalValue;

    public static implicit operator int(ModifiableValue other) => other.finalIntValue;

    public ModifiableValue()
    {
        baseValue = 0;
    }

    public ModifiableValue(float value)
    {
        baseValue = value;
    }

    public void resetModifiers()
    {
        multiplier = 1;
        additive = 0;
        additiveBase = 0;
    }

}
