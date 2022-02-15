using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePerson : BattleUnit
{
    protected Animator animator;
    public float knockbackMultiplier = 20;
    private int comboCount = 0;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x));
        animator.SetBool("Facing Right", facingRight);
    }

    public void WeakAttack()
    {
        canMove = false;
        animator.SetBool("Attack Cancelable", false);
        lastAttack = new Attack(
            1,                              // Base Power
            1,                              // Base Speed
            40,                              // Base Knockback Power
            new Vector2(2f, 0.5f),          // Base Knockback Angle
            0.5f                            // Base Hitstun
            );
    }

    public void StrongAttack()
    {
        canMove = false;
        animator.SetBool("Attack Cancelable", false);
        lastAttack = new Attack(
            3,                          // Base Power
            1,                          // Base Speed
            100,                         // Base Knockback Power
            new Vector2(2, 0.5f),       // Base Knockback Angle
            1                           // Base Hitstun
            );
    }

    public override void HitTarget(BattleUnit target)
    {
        if (gameObject.tag == target.tag) { return; }
        if (lastAttack != null)
        {
            animator.SetBool("Attack Cancelable", true);
            int knockbackDir = facingRight ? 1 : -1;
            Vector2 knockbackAngle = new Vector2(lastAttack.knockbackAngle.x * knockbackDir * 1, lastAttack.knockbackAngle.y * 1);
            
            target.TakeDamage(
                this,
                lastAttack.baseAttack,
                lastAttack.baseKnockback * 1 + (knockbackMultiplier * comboCount),
                knockbackAngle,
                lastAttack.baseHitstun
                );

            comboCount++;
        }
    }

    public override void Jump(float direction)
    {
        if (grounded && CanMove())
        {
            velocity.x = walkSpeed * direction;
            velocity.y = CalculateJumpPower();
            EndCombo();
        }
    }

    public override void Attack01()
    {
        if (!stunned && grounded)
        {
            animator.SetTrigger("Weak Attack");
        }
    }

    public override void Attack02()
    {
        if (!stunned && grounded)
        {
            animator.SetTrigger("Strong Attack");
        }
    }

    public void EndCombo()
    {
        comboCount = 0;
    }

    protected override void OnDefeated()
    {
        animator.SetTrigger("Death");
    }

    protected override void OnDamaged()
    {
        throw new System.NotImplementedException();
    }
}
