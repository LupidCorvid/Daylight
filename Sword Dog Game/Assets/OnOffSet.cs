using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffSet : MonoBehaviour
{
    public Image onButton;
    public Image offButton;

    public Sprite onActiveSprite;
    public Sprite onInactiveSprite;

    public Sprite offActiveSprite;
    public Sprite offInactiveSprite;

    public bool state;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    public void TurnOff()
    {
        onButton.sprite = onInactiveSprite;
        offButton.sprite = offActiveSprite;
        state = false;
    }

    
}
