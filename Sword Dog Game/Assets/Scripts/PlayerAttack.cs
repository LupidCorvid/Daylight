using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    public int attackCombo = 0;
    public int maxCombo = 2;
    public bool isAttacking = false, canAttack = true;
    public bool isParrying = false;
    public float attackCooldown = 0.0f;
    public PlayerMovement pMovement;
    public Transform parryTrackerLocation;
    public float parryStaminaCost = 2.5f;
    public bool downSwing = false;

    public Vector2 parryAimDir;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputReader.inputs.actions["Block"].IsPressed())
        {
            
        }
        if (!pMovement.blackout && !PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open) // && not paused(?)
        {
            float yInput = InputReader.inputs.actions["Move"].ReadValue<Vector2>().y;

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

            anim.SetFloat("attack_direction", yInput);

            // attack input detection + combo tracking
            if (InputReader.inputs.actions["Attack"].WasPressedThisFrame() && !isParrying && canAttack /*&& attackCombo < maxCombo*/ && !PauseScreen.paused && !PlayerHealth.dead && SwordFollow.instance?.activeInHierarchy == true)
            {
                // set attack direction + context
                if (!isAttacking)
                {
                    anim.SetFloat("attack_direction", yInput);
                    if (yInput < 0)
                    {
                        Debug.Log("downswing"); downSwing = true;
                    }
                    SwordFollow.sword.speed = 2;
                    attackCombo = 0;
                    attackCooldown = 0;

                    // Add air resistance for midair attacks
                    if (!pMovement.isGrounded)
                    {
                        GetComponent<Rigidbody2D>().drag = 20;
                    }
                }

                anim.SetFloat("attackSpeed", pMovement.entityBase.attackSpeed);
                
                isAttacking = true;
                if (!(pMovement.isDashing && !pMovement.canDash))
                {
                    attackCombo = attackCombo % 2 + 1;
                    anim.SetTrigger("attack" + attackCombo);
                }
            }

            //Debug.Log(!isAttacking && pMovement.stamina >= parryStaminaCost);
            

            //Parry input
            if(InputReader.inputs.actions["Block"].IsPressed() && !isAttacking && pMovement.stamina >= parryStaminaCost && !pMovement.swordAnimControl)
            {
                if (!isParrying)
                    pMovement.entityBase.moveSpeed.multiplier *= .5f;
                //float angle = (Vector3.Angle(Vector3.right, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)) * Mathf.Deg2Rad;
                Vector2 inputVector = Vector2.zero;
                if (InputReader.inputs.currentControlScheme == "Keyboard")
                    inputVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

                //Vector2 inputVector = InputReader.inputs.actions["AimDir"].ReadValue<Vector2>();
                if (InputReader.inputs.currentControlScheme == "Controller")
                {
                    inputVector = InputReader.inputs.actions["AimDir"].ReadValue<Vector2>();
                }
                if (inputVector != Vector2.zero)
                    parryAimDir = inputVector;

                float angle = Mathf.Atan2(parryAimDir.y, parryAimDir.x);
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
                StopParry();
            }
            
            // perhaps useful in the future for preventing sprint/jump from interrupting attack
            anim.SetBool("attacking", isAttacking);
        }
    }

    private void StopParry()
    {
        if (isParrying)
            pMovement.entityBase.moveSpeed.multiplier /= .5f;
        isParrying = false;
        pMovement.canTurn = true;
        pMovement.canSprint = true;
        pMovement.stopStaminaRefill = false;
    }

    private void NoDrag() {
        var rb = GetComponent<Rigidbody2D>();
        rb.drag = 0;
        if (downSwing && !pMovement.isGrounded)
        {
            rb.gravityScale = 12;
            downSwing = false;
            Debug.Log("highgrav");
        }
    }

    // stops attacks -- called from animation events in return states
    private void StopAttack()
    {
        StopAttack(true);
    }

    private void StopAttack(bool resetCombo = false)
    {
        NoDrag();
        if (anim == null)
            return;
        isAttacking = false;
        if (resetCombo)
            ResetCombo();
        // attackCooldown = cooldownLength;
        anim.ResetTrigger("exit_trot");
        pMovement.finishedReverseTurnThisFrame = false;
        pMovement.isDashing = false;
    }

    private void StopAttackKeepCombo()
    {
        StopAttack(false);
    }

    private void ResetCombo()
    {
        attackCombo = 0;
        for (int i = 1; i <= maxCombo; i++)
        {
            anim.ResetTrigger("attack" + i);
        }
    }
}
