using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResults {

    // Information about the battle to pass back to the map scene.

    public GameCharacter winner;
    public bool retreated;

    public BattleResults(GameCharacter winner, bool retreated)
    {
        this.winner = winner;
        this.retreated = retreated;
    }

}
