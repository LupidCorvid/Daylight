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

    public void Set(bool newVal)
    {
        state = newVal;
        if (state)
            setEnabledSprites();
        else
            setDisabledSprites();
    }

    public void TurnOn()
    {
        setEnabledSprites();

        toggled?.Invoke();
    }

    public void setEnabledSprites()
    {
        onButton.sprite = onActiveSprite;
        offButton.sprite = offInactiveSprite;
        state = true;
    }

    public void TurnOff()
    {
        setDisabledSprites();

        toggled?.Invoke();
    }

    public void setDisabledSprites()
    {
        onButton.sprite = onInactiveSprite;
        offButton.sprite = offActiveSprite;
        state = false;
    }

    
}
