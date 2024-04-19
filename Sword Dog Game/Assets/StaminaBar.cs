using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public Image fillImage;
    public Animator animator;
    private bool pulsing;

    void Update()
    {
        //if(Player.controller != null)
        //    slider.value = Player.controller.stamina / Player.controller.maxStamina;
        
        fillImage ??= GetComponent<Image>();
        animator ??= GetComponent<Animator>();

        if (Player.controller != null)
        {
            fillImage.fillAmount = Player.controller.stamina / Player.controller.maxStamina;
            if (Player.controller.isSprinting && !pulsing)
            {
                animator.SetTrigger("start");
                pulsing = true;
            }
            if (!Player.controller.isSprinting && pulsing)
            {
                animator.SetTrigger("stop");
                pulsing = false;
            }
        }

        
    }
}
