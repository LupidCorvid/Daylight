using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    public int attackCombo = 0;
    public bool isAttacking = false, canAttack = true;
    public bool isParrying = false;
    public float attackCooldown = 0.0f;
    public PlayerMovement pMovement;
    public Transform parryTrackerLocation;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open) // && not paused(?)
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
            if (Input.GetMouseButtonDown(0) && !isParrying && canAttack && attackCombo < 3 && !PauseScreen.paused && !PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open && SwordFollow.instance?.activeInHierarchy == true)
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
            //Parry input
            if(Input.GetMouseButton(1) && !isAttacking && pMovement.stamina >= .75f)
            {
                if (!isParrying)
                    pMovement.entityBase.moveSpeed.multiplier *= .5f;
                //float angle = (Vector3.Angle(Vector3.right, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)) * Mathf.Deg2Rad;
                Vector2 inputVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float angle = Mathf.Atan2(inputVector.y, inputVector.x);
                Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position, default);
                int neg = (pMovement.facingRight) ? 1 : -1;
                //angle = Mathf.Clamp(angle, -Mathf.PI/2, Mathf.PI/2));
                if (neg == 1)
                    angle = Mathf.Clamp(angle, -Mathf.PI / 2, Mathf.PI / 2);
                else
                {
                    if (angle < Mathf.PI / 2 && angle > 0)
                        angle = Mathf.PI / 2;
                    if (angle > -Mathf.PI / 2 && angle < 0)
                        angle = -Mathf.PI / 2;
                }
                parryTrackerLocation.localPosition = new Vector3(1.5f * Mathf.Cos(angle) * neg, Mathf.Sin(angle), 0) * 1.5f;
                parryTrackerLocation.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 90);

                isParrying = true;
                pMovement.canTurn = false;
                pMovement.canSprint = false;
                pMovement.stopStaminaRefill = true;

            }
            else
            {
                if (isParrying)
                    pMovement.entityBase.moveSpeed.multiplier /= .5f;
                isParrying = false;
                pMovement.canTurn = true;
                pMovement.canSprint = true;
                pMovement.stopStaminaRefill = false;
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
        if (anim == null)
            return;
        // attackCooldown = cooldownLength;
        for (int i = 1; i <= 3; i++)
        {
            anim.ResetTrigger("attack" + i);
        }
        anim.ResetTrigger("exit_trot");
    }
}
