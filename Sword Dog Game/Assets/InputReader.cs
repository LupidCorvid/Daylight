using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{

    public static PlayerInput inputs;

    void Awake()
    {
        PlayerInput newInput = GetComponent<PlayerInput>();
        if (newInput != null && inputs == null)
            inputs = newInput;

        inputs.ActivateInput();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
