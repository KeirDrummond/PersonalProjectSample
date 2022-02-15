using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData
{
    public GameCharacter attacker;
    public GameCharacter defender;
    public MapData map;

    public BattleData(ref GameCharacter attacker, ref GameCharacter defender, MapData map)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.map = map;
    }

}
