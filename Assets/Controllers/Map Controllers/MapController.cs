using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapController
{
    // Actions that a player can do on the map. Human players and AI players use the functions here to act.

    // What team does this player control?
    [SerializeField]
    protected Team team;

    protected List<GameCharacter> myUnits;

    // The selected unit attempts to wait at the location. Returns False if not possible.
    protected bool WaitAtLocation(GameCharacter unit, Vector2Int position)
    {
        if (unit.team != team) { return false; }

        unit.mapUnit.Wait(position);
        unit.mapUnit.EndOfAction();
        return true;
    }

    // The selected unit attempts to attack a target from a location. Returns False if not possible.
    protected bool AttackTarget(GameCharacter unit, GameCharacter target, Vector2Int position)
    {
        if (unit.team != team) { return false; }

        unit.mapUnit.SetPosition(position);
        unit.mapUnit.EndOfAction();
        GameManager.Instance.GetMapManager().StartBattle((Vector3Int)unit.mapUnit.GetPosition(), ref unit, ref target);
        return true;
    }


    // Anything that needs to be done before the turn is ended i.e. character abilities.
    protected void EndTurn()
    {
        GameManager.Instance.EndTurn();
    }

    public virtual void StartOfTurn()
    {
        ResetUnits();
    }

    protected void ResetUnits()
    {
        foreach(GameCharacter unit in myUnits)
        {
            unit.mapUnit.NewTurn();
        }
    }
}
