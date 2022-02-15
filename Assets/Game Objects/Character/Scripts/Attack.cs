using UnityEngine;

public class Attack {

    public float baseAttack = 0;
    public float baseSpeed = 0;
    public float baseKnockback = 0;
    public Vector2 knockbackAngle = Vector2.zero;
    public float baseHitstun = 0;

    public Attack(float baseAttack, float baseSpeed, float baseKnockback, Vector2 knockbackAngle, float baseHitstun)
    {
        this.baseAttack = baseAttack;
        this.baseSpeed = baseSpeed;
        this.baseKnockback = baseKnockback;
        this.knockbackAngle = knockbackAngle;
        this.baseHitstun = baseHitstun;
    }
}