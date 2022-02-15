using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Character", menuName = "ScriptableObjects/MapCharacter", order = 1)]
public class CharacterData : ScriptableObject
{
    public string unitName;
    public int health;
    public int movement;
    public Sprite sprite;

    public BattleUnit battleUnit;
}
