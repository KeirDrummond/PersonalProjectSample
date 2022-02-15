using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 1)]
public class MapData : ScriptableObject
{
    public Tilemap collision;
    public Tilemap platforms;
    public TextAsset AStarGraph;
    public Vector3 attackerSpawn;
    public Vector3 defenderSpawn;
    public int minXBound;
    public int maxXBound;
}
