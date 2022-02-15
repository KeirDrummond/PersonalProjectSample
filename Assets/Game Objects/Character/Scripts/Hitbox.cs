using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public BattleUnit character;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hurtbox" && other.transform.parent != gameObject.transform.parent)
        {
            BattleUnit otherCharacter = other.gameObject.GetComponentInParent<BattleUnit>();
            character.HitTarget(otherCharacter);
        }
    }
}
