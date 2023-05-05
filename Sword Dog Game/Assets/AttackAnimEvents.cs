using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimEvents : MonoBehaviour
{
    public SwordTip sword;

    // Start is called before the first frame update
    void Start()
    {
        sword = GameObject.Find("Tip")?.GetComponent<SwordTip>();
    }

    public void ClearCollisions()
    {
        sword ??= GameObject.Find("Tip")?.GetComponent<SwordTip>();
        sword.ClearCollisions();
    }
}
