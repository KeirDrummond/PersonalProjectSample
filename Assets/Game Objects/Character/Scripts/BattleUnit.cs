using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleUnit : MonoBehaviour
{
    [Header("Grounded Movement")]
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public float groundAcceleration;
    public float groundDeceleration;
    public float jumpHeight = 4;

    [Header("Air Movement")]
    public float airSpeed = 1;
    public float airAcceleration;
    public float airDeceleration;

    [Header("Character stats")]
    public float maxHealth = 10;
    public float currentHealth = 10;

    public float mass = 1;

    [SerializeField]
    protected HealthBar healthBar;

    BattlePlayerController controller;
    float moveX = 0;

    protected bool grounded;
    protected bool sprinting = false ;

    protected bool canMove = true;
    protected bool facingRight = true;

    Controller2D controller2D;
    protected Vector2 velocity;
    protected Vector2 targetVelocity;

    protected Attack lastAttack;

    protected Timer stunTimer;
    protected bool stunned = false;

    public Team team;
    public bool retreated;
    public bool defeated;

    private AIUnit AIUnit;

    protected virtual void Awake()
    {
        controller2D = GetComponent<Controller2D>();
        AIUnit = GetComponentInChildren<AIUnit>();
        retreated = false;
        defeated = false;
        stunTimer = new Timer(0, false);
    }

    protected virtual void Start()
    {
        UpdateHealth(currentHealth, maxHealth);
    }

    protected virtual void FixedUpdate()
    {
        velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
        if (controller2D.collisions.below && velocity.y <= 0)
        {
            velocity.y = 0;
            grounded = true;
        }
        else { grounded = false; }

        if (grounded) { GroundedMovement(); }
        else { AirMovement(); }

        BattleManager battleManager = GameManager.Instance.GetBattleManager();
        if (transform.position.x < battleManager.GetRetreatLine())
        {
            Retreat();
        }

        Pushbox pushBox = GetComponentInChildren<Pushbox>();
        if (pushBox) {
            Vector2 push = pushBox.PushBox(false);
            velocity += (push / mass) * 3 * Time.fixedDeltaTime;

            push = pushBox.PushBox(true);
            if (push != Vector2.zero)
            {
                float dir = Mathf.Sign(push.x);
                if (dir == 1) { velocity.x = Mathf.Max(0, push.x); }
                else { velocity.x = Mathf.Min(0, push.x); }
            }
        }

        if (controller2D.collisions.left)
        {
            velocity.x = Mathf.Max(0, velocity.x);
        }
        if (controller2D.collisions.right)
        {
            velocity.x = Mathf.Min(0, velocity.x);
        }

        controller2D.Move(velocity);

        if (grounded)
        {
            if (targetVelocity.x > 0.01f) { facingRight = true; }
            else if (targetVelocity.x < -0.01f) { facingRight = false; }
        }

        UpdateTimers();
    }

    private void GroundedMovement()
    {
        targetVelocity.x = sprinting ? moveX * runSpeed : moveX * walkSpeed;
        if (!CanMove()) { targetVelocity = Vector2.zero; }
        float velDif = Mathf.Abs(velocity.x - targetVelocity.x);
        if (velocity.x == 0)
        {
            int dir = targetVelocity.x >= 0 ? 1 : -1;
            velocity.x += Mathf.Min(groundAcceleration * Time.deltaTime, velDif) * dir;
        }
        else if (velocity.x > 0)
        {
            if (targetVelocity.x > velocity.x) { velocity.x += Mathf.Min(groundAcceleration * Time.deltaTime, velDif); }
            else if (targetVelocity.x < velocity.x) { velocity.x -= Mathf.Min(groundDeceleration * Time.deltaTime, velDif); }
        }
        else if (velocity.x < 0)
        {
            if (targetVelocity.x < velocity.x) { velocity.x -= Mathf.Min(groundAcceleration * Time.deltaTime, velDif); }
            else if (targetVelocity.x > velocity.x) { velocity.x += Mathf.Min(groundDeceleration * Time.deltaTime, velDif); }
        }
    }

    private void AirMovement()
    {
        targetVelocity.x = moveX * airSpeed;
        if (!CanMove()) { targetVelocity = Vector2.zero; }
        float velDif = Mathf.Abs(velocity.x - targetVelocity.x);
        if (velocity.x == 0)
        {
            int dir = targetVelocity.x >= 0 ? 1 : -1;
            velocity.x += Mathf.Min(airAcceleration * Time.deltaTime, velDif) * dir;
        }
        else if (velocity.x > 0)
        {
            if (targetVelocity.x > velocity.x) { velocity.x += Mathf.Min(airAcceleration * Time.deltaTime, velDif); }
            else if (targetVelocity.x < velocity.x) { velocity.x -= Mathf.Min(airDeceleration * Time.deltaTime, velDif); }
        }
        else if (velocity.x < 0)
        {
            if (targetVelocity.x < velocity.x) { velocity.x -= Mathf.Min(airAcceleration * Time.deltaTime, velDif); }
            else if (targetVelocity.x > velocity.x) { velocity.x += Mathf.Min(airDeceleration * Time.deltaTime, velDif); }
        }
    }

    public void Possess(BattlePlayerController newController)
    {
        controller = newController;
        AIUnit.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        healthBar = GameManager.Instance.GetBattleManager().GetPlayerHealthBar();
    }

    public BattlePlayerController GetController() { return controller; }

    public void ControlCharacter(Vector2 move)
    {
        moveX = move.x;
    }

    public void MoveCharacterX(float moveX)
    {
        velocity.x += moveX;
    }

    protected void Knockback(float knockbackPower, Vector2 knockbackAngle)
    {
        float kbx = knockbackAngle.normalized.x * knockbackPower / mass;
        float kby = knockbackAngle.normalized.y * knockbackPower / mass;

        velocity.x += kbx;
        velocity.y += kby;
    }

    protected void AddForce(Vector2 force)
    {
        Vector2 acceleration = force / mass;
        velocity += acceleration;
    }

    protected bool CanMove()
    {
        if (stunned) { return false; }
        if (!canMove) { return false; }
        return true;
    }

    public virtual void Jump(float direction)
    {
        if (grounded && CanMove())
        {
            velocity.x = walkSpeed * direction;
            velocity.y = CalculateJumpPower();
        }
    }

    protected float CalculateJumpPower()
    {
        return Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight); ;
    }

    public abstract void Attack01();
    public abstract void Attack02();

    public virtual void HitTarget(BattleUnit target)
    {
        Debug.Log("Hit " + target);
        if (gameObject.tag == target.tag) { return; }
        if (lastAttack != null)
        {
            int knockbackDir = facingRight ? 1 : -1;
            Vector2 knockbackAngle = new Vector2(lastAttack.knockbackAngle.x * knockbackDir, lastAttack.knockbackAngle.y);

            target.TakeDamage(
                this,
                lastAttack.baseAttack,
                lastAttack.baseKnockback,
                knockbackAngle,
                lastAttack.baseHitstun
                );
        }
    }

    public void TakeDamage(BattleUnit source, float damage, float knockbackPower, Vector2 knockbackAngle, float hitStun)
    {
        if (currentHealth > 0)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            AddForce(knockbackAngle.normalized * knockbackPower);
            stunned = true;
            stunTimer.SetNewDuration(hitStun, true);
            UpdateHealth(currentHealth, maxHealth);
            if (currentHealth > 0)
            {
            }
            else
            {
                Defeated();
            }
        }
    }

    protected abstract void OnDamaged();

    protected abstract void OnDefeated();

    private void Defeated()
    {
        defeated = true;
        GameManager.Instance.GetBattleManager().UnitDefeated();
        OnDefeated();
    }

    private void Retreat()
    {
        retreated = true;
        GameManager.Instance.GetBattleManager().UnitRetreated();
    }

    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.UpdateHealth(maxHealth, currentHealth);
    }

    public void EnableMove()
    {
        canMove = true;
    }

    public void DisableMove()
    {
        canMove = false;
    }

    private void UpdateTimers()
    {
        if (stunned) {
            if (stunTimer.CountDown(Time.deltaTime))
            {
                stunned = false;
            }
        }
    }

}