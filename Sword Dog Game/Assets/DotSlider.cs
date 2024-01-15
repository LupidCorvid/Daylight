using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DotSlider : MonoBehaviour
{
    List<Button> dots = new List<Button>();

    public float setValue = 1;

    public Action changed;

    // Start is called before the first frame update
    void Start()
    {
        dots.AddRange(GetComponentsInChildren<Button>());

        for(int i = 0; i < dots.Count; i++)
        {
            dots[i].onClick.AddListener(() => DotClicked(i));
        }
        
    }

    public void DotClicked(int index)
    {
        setValue = index * 1f / dots.Count;

        changed?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
