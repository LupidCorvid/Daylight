using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DotSlider : MonoBehaviour
{
    List<Button> dots = new List<Button>();

    public float value = 1;

    public Action changed;

    public Color FillColor;
    public Color EmptyColor;

    // Start is called before the first frame update
    void Start()
    {
        dots.AddRange(GetComponentsInChildren<Button>());

        for(int i = 0; i < dots.Count; i++)
        {
            int index = i; //This is needed so that the lambda expression doesn't have it's input values all be the final value of i
            dots[i].onClick.AddListener(() => DotClicked(index));
        }
        
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        for (int i = 0; i < dots.Count; i++)
        {
            if (i * 1f / (dots.Count - 1) <= value)
                dots[i].image.color = FillColor;
            else
                dots[i].image.color = EmptyColor;
        }
    }

    public void DotClicked(int index)
    {
        value = index * 1f / (dots.Count - 1);

        changed?.Invoke();

        for (int i = 0; i < dots.Count; i++)
        {
            if(i <= index)
                dots[i].image.color = FillColor;
            else
                dots[i].image.color = EmptyColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
