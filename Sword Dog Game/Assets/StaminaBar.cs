using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Update()
    {
        if(Player.controller != null)
            slider.value = Player.controller.stamina / Player.controller.maxStamina;
    }
}
