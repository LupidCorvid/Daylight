using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAttackTrigger : MonoBehaviour
{
    public EnemyBase enemy;
    
    public void triggerDamage()
    {
        enemy?.ai?.applyAttackDamage();
    }
}