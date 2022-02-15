using System;
using UnityEngine;

public class GameCharacter
{
    public GameCharacter(CharacterData characterData, MapUnit prefab, Transform parent, Vector2Int startPosition, Team team)
    {
        unitName = characterData.unitName;
        health = characterData.health;
        movement = characterData.movement;
        active = true;

        this.team = team;

        battleUnit = characterData.battleUnit;

        mapUnit = GameObject.Instantiate(prefab, parent);
        mapUnit.SetSprite(characterData.sprite);
        mapUnit.SetPosition(startPosition);
    }

    public string unitName;
    public int health;
    public int movement;

    public bool active;

    public Team team;

    public MapUnit mapUnit;
    public BattleUnit battleUnit;
}
