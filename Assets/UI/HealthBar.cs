using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField]
    protected Transform healthBar;
    [SerializeField]
    protected Transform greenHealth;
    
    public abstract void UpdateHealth(float maxHealth, float currentHealth);
}
