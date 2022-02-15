using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : BattleUnit
{
    Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
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
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x));
    }
    public override void Attack01()
    {
        if (!stunned)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Attack()
    {
        canMove = false;
        lastAttack = new Attack(
            1,                              // Base Power
            1,                              // Base Speed
            20,                              // Base Knockback Power
            new Vector2(2f, 0.5f),          // Base Knockback Angle
            0.5f                            // Base Hitstun
            );
    }

    public override void Attack02()
    {
        throw new System.NotImplementedException();
    }

    public override void Jump(float direction)
    {
        base.Jump(direction);
        animator.SetTrigger("Jump");
    }

    protected override void OnDefeated()
    {
        animator.SetTrigger("Death");
    }

    protected override void OnDamaged()
    {
        animator.SetTrigger("Hurt");
    }
}
