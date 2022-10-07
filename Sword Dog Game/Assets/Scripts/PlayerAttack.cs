using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private int attackCombo = 0;
    private bool isAttacking = false, canAttack = true;
    [SerializeField] private float cooldownLength = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && attackCombo < 3)
        {
            isAttacking = true;
            attackCombo++;

            anim.SetTrigger("attack" + attackCombo);
        }

        anim.SetBool("attacking", isAttacking);
    }

    private void StopAttack()
    {
        attackCombo = 0;
        isAttacking = false;
        StopCoroutine("AttackCooldown");
        StartCoroutine(AttackCooldown(cooldownLength));
        
        for (int i = 1; i <= 3; i++)
        {
            anim.ResetTrigger("attack" + i);
        }
    }

    IEnumerator AttackCooldown(float seconds)
    {
        canAttack = false;
        yield return new WaitForSeconds(seconds);
        canAttack = true;
    }
}
