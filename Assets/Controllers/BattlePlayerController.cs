using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController
{
    public BattleUnit character;
    public GameObject healthBarPrefab;
    public GameObject healthBar;

    // Update is called once per frame
    public void Update()
    {
        if (character != null)
        {
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxis("Horizontal");

            character.ControlCharacter(move);

            if (Input.GetButtonDown("Jump"))
            {
                character.Jump(move.x);
            }

            if (Input.GetButtonDown("Weak Attack"))
            {
                character.Attack01();
            }

            if (Input.GetButtonDown("Strong Attack"))
            {
                character.Attack02();
            }
        }
    }

    public void ControlCharacter(BattleUnit unit)
    {
        character = unit;
        character.Possess(this);
    }
}
