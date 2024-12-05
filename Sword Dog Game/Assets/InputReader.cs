using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public enum DeviceType {
        Keyboard, Xbox, Switch, PlayStation, SteamDeck
    }
    
    public static List<TMP_SpriteAsset> spriteAssets;

    public List<TMP_SpriteAsset> listOfSpriteAssets;

    public static DeviceType deviceType;

    public static PlayerInput inputs;

    void Awake()
    {
        spriteAssets = listOfSpriteAssets;
        PlayerInput newInput = GetComponent<PlayerInput>();
        if ((newInput != null && inputs == null) || inputs.gameObject == null)
            inputs = newInput;
        inputs.ActivateInput();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
