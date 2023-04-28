using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Update()
    {
        if(PlayerMovement.controller != null)
            slider.value = PlayerMovement.controller.stamina / PlayerMovement.controller.maxStamina;
    }
}
