using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OnOffSet : MonoBehaviour
{
    public Image onButton;
    public Image offButton;

    public Sprite onActiveSprite;
    public Sprite onInactiveSprite;

    public Sprite offActiveSprite;
    public Sprite offInactiveSprite;

    public bool state;

    public Action toggled;

    // Start is called before the first frame update
    void Start()
    {
        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateState()
    {
        if (state)
            TurnOn();
        else
            TurnOff();
    }

    public void TurnOn()
    {
        onButton.sprite = onActiveSprite;
        offButton.sprite = offInactiveSprite;
        state = true;

        toggled?.Invoke();
    }

    public void TurnOff()
    {
        onButton.sprite = onInactiveSprite;
        offButton.sprite = offActiveSprite;
        state = false;

        toggled?.Invoke();
    }

    
}
