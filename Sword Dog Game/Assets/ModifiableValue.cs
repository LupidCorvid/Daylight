using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifiableValue
{
    public float baseValue = 1;

    //Only add or subtract to this
    public float baseMultiplier = 1;

    public float additive = 0;

    public float additiveBase = 0;

    //Only multiply or divide this
    public float multiplier = 1;

    public float finalValue
    {
        get
        {
            if (setZeroSources == 0)
                return (baseValue + additiveBase) * baseMultiplier * multiplier + additive;
            else
                return 0;
        }
    }

    //Since you cannot undo * 0, increment this if you want to effectively multiply it by 0 and be able to undo it
    public int setZeroSources = 0;

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
        baseMultiplier = 1;
        additive = 0;
        additiveBase = 0;
        multiplier = 1;
    }

}
