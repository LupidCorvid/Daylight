using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    private Vector3 startPos;
    public float yOffset;
    private Animator anim;
    public bool tutorial;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + new Vector3(0, yOffset, 0);
        if (tutorial && anim.GetFloat("Speed") < 0 && anim.GetBool("OpenBubble"))
        {
            anim.ResetTrigger("OpenBubble");
        }
    }

    void BackToWiggle()
    {
        if (anim.GetFloat("Speed") < 0) {
            anim.Play("Wiggle");
            anim.SetFloat("Speed", 1);
        }
    }
}
