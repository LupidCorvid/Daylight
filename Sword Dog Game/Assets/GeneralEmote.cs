using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEmote : MonoBehaviour
{
    public Animator anim;
    public float lifeTime = 1;
    public float startTime;
    public int type;

    private void Start()
    {
        startTime = Time.time;
        anim.SetFloat("Type", type);
    }

    private void Update()
    {
        if (startTime + lifeTime <= Time.time)
            anim.SetTrigger("Finish");
    }
}
