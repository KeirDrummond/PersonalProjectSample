using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    public override void UpdateHealth(float maxHealth, float currentHealth)
    {
        float fillPercent = currentHealth / maxHealth;
        greenHealth.transform.localScale = new Vector3(fillPercent, 1, 1);
    }
}
