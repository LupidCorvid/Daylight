using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public Image fillImage;

    void Update()
    {
        //if(Player.controller != null)
        //    slider.value = Player.controller.stamina / Player.controller.maxStamina;
        
        fillImage ??= GetComponent<Image>();

        if (Player.controller != null)
            fillImage.fillAmount = Player.controller.stamina / Player.controller.maxStamina;

        
    }
}
