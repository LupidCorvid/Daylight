using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    public int attackCombo = 0;
    public bool isAttacking = false, canAttack = true;
    public float attackCooldown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerHealth.dead && !CutsceneController.cutsceneStopMovement) // && not paused(?)
        {
            float yInput = Input.GetAxisRaw("Vertical");

            // attack cooldown
            if (attackCooldown > 0)
            {
                canAttack = false;
                attackCooldown -= Time.deltaTime;
            }
            if (attackCooldown <= 0 && !isAttacking)
            {
                attackCombo = 0;
                canAttack = true;
                attackCooldown = 0;
            }

            // attack input detection + combo tracking
            if (Input.GetMouseButtonDown(0) && canAttack && attackCombo < 3)
            {
                // set attack direction + context
                if (!isAttacking)
                {
                    anim.SetFloat("attack_direction", yInput);
                    SwordFollow.sword.speed = 2;
                }
                isAttacking = true;
                attackCombo++;
                anim.SetTrigger("attack" + attackCombo);
            }
            
            // perhaps useful in the future for preventing sprint/jump from interrupting attack
            anim.SetBool("attacking", isAttacking);
        }
    }

    // stops attacks -- called from animation events in return states
    private void StopAttack()
    {
        attackCombo = 0;
        isAttacking = false;
        // attackCooldown = cooldownLength;
        for (int i = 1; i <= 3; i++)
        {
            anim.ResetTrigger("attack" + i);
        }
        anim.ResetTrigger("exit_trot");
    }
}
