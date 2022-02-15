using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code from https://www.youtube.com/watch?v=alU04hvz6L4

public class MapPathNode
{
    private MapGrid<MapPathNode> grid;
    public int x;
    public int y;

    public int gCost; // Walking Cost from the Start Node
    public int hCost; // Heuristic Cost to reach End Node
    public int fCost; // G + H

    public bool isWalkable;
    public int movementCost;
    public MapPathNode cameFromNode;

    public MapPathNode(MapGrid<MapPathNode> grid, int x, int y, bool isWalkable, int movementCost)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.movementCost = movementCost;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

}
