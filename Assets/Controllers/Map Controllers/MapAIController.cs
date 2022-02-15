using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAIController : MapController
{
    public MapAIController(Team team)
    {
        this.team = team;
        myUnits = GetMyUnitsOnField();
    }

    // The AI first picks a unit to perform an action with. If there are no units remaining, the turn will end.
    public void MyTurn()
    {
        bool action = false;
        for (int i = 0; i < myUnits.Count; i++)
        {
            if (myUnits[i].active && !myUnits[i].mapUnit.HasUnitedActed())
            {
                GameCharacter unit = myUnits[i];
                PerformAction(unit);
                action = true;
                break;
            }
        }
        if (!action) { EndTurn(); }
    }

    // The selected unit will attempt to attack the closest enemy unit. If there are no targets in range, it will move towards it.
    private void PerformAction(GameCharacter unit)
    {
        MapPathing pathing = GameManager.Instance.GetMapManager().GetPathing();
        GameCharacter target = GetNearestEnemy(unit.mapUnit.GetPosition());
        if (target == null) { return; }
        int distance = GetPathDistance(unit.mapUnit.GetPosition(), target.mapUnit.GetPosition());
        if (distance > unit.movement)
        {
            MoveTowardsTarget(unit, target);
        }
        else
        {
            AttackTarget(unit, target);
        }
    }

    private void MoveTowardsTarget(GameCharacter unit, GameCharacter target)
    {
        MapPathing pathing = GameManager.Instance.GetMapManager().GetPathing();
        Vector2Int posA = unit.mapUnit.GetPosition();
        Vector2Int posB = target.mapUnit.GetPosition();
        List<MapPathNode> nodes = pathing.FindPath(posA.x, posA.y, posB.x, posB.y);

        Vector2Int destination = posA;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].gCost > unit.movement)
            {
                if (GameManager.Instance.GetMapManager().GetUnitAtPosition(destination) != null) { }
                destination = new Vector2Int(nodes[i].cameFromNode.x, nodes[i].cameFromNode.y);
                break;
            }
        }        

        WaitAtLocation(unit, destination);
    }

    private void AttackTarget(GameCharacter unit, GameCharacter target)
    {
        MapPathing pathing = GameManager.Instance.GetMapManager().GetPathing();
        Vector2Int posA = unit.mapUnit.GetPosition();
        Vector2Int posB = target.mapUnit.GetPosition();
        List<MapPathNode> nodes = pathing.FindPath(posA.x, posA.y, posB.x, posB.y);

        MapPathNode desNode = nodes[nodes.Count - 1].cameFromNode;
        Vector2Int destination = new Vector2Int(desNode.x, desNode.y);
        AttackTarget(unit, target, destination);
    }

    private GameCharacter GetNearestEnemy(Vector2Int position)
    {
        List<GameCharacter> enemies = GetUnitsOnFieldOnTeam(Team.PLAYER);
        if (enemies.Count == 0) { return null; }
        GameCharacter nearestEnemy = enemies[0];
        int shortestDistance = GetPathDistance(position, nearestEnemy.mapUnit.GetPosition());
        for (int i = 1; i < enemies.Count; i++)
        {
            int targetDistance = GetPathDistance(position, enemies[i].mapUnit.GetPosition());
            if (targetDistance < shortestDistance)
            {
                nearestEnemy = enemies[i];
                shortestDistance = targetDistance;
            }
        }
        return nearestEnemy;
    }

    int GetPathDistance(Vector2Int pos1, Vector2Int pos2)
    {
        MapPathing pathing = GameManager.Instance.GetMapManager().GetPathing();

        return pathing.GetDistance(pos1.x, pos1.y, pos2.x, pos2.y);
    }

    List<GameCharacter> GetMyUnitsOnField()
    {
        List<GameCharacter> allUnits = new List<GameCharacter>();
        List<GameCharacter> units = new List<GameCharacter>();

        allUnits = GameManager.Instance.GetCharacterList();
        foreach(GameCharacter unit in allUnits)
        {
            if (unit.team == team)
            {
                units.Add(unit);
            }
        }
        return units;
    }

    List<GameCharacter> GetUnitsOnFieldOnTeam(Team team)
    {
        List<GameCharacter> allUnits = new List<GameCharacter>();
        List<GameCharacter> units = new List<GameCharacter>();

        allUnits = GameManager.Instance.GetCharacterList();
        foreach (GameCharacter unit in allUnits)
        {
            if (unit.team == team)
            {
                units.Add(unit);
            }
        }
        return units;
    }
}
