using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void interact(GameObject user);

    public GameObject gameObject
    {
        get;
    }
}
