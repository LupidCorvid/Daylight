using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BackToWiggle()
    {
        Animator anim = GetComponent<Animator>();
        if (anim.GetFloat("Speed") < 0) {
            anim.Play("Wiggle");
            anim.SetFloat("Speed", 1);
        }
    }
}
