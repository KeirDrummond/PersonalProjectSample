using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : BattleUnit
{
    public void StandardAttack()
    {
        lastAttack = new Attack(
            1,                  // Base Power
            1,                  // Base Speed
            30,                 // Base Knockback Power
            new Vector2(1,1),   // Base Knockback Angle
            1                   // Base Hitstun
            );
    }

    public override void Attack01()
    {
        if (!stunned)
        {
            //animator.SetTrigger("Attack");
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            healthBar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
            healthBar.transform.localRotation = Quaternion.Euler(0, -180, 0);
        }
    }

    public override void Attack02()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDefeated()
    {
    }

    protected override void OnDamaged()
    {
    }
}
