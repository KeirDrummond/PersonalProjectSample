using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Code from https://www.youtube.com/watch?v=alU04hvz6L4

public class MapGrid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;

    public MapGrid(Tilemap tilemap, float cellSize, Func<MapGrid<TGridObject>, int, int, int, TGridObject> createGridObject)
    {
        width = tilemap.size.x;
        height = tilemap.size.y;
        this.cellSize = cellSize;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                int m = 1;
                MapTile tile = (MapTile)tilemap.GetTile(new Vector3Int(x, y));
                if (tile)
                {
                    m = tile.movementCost;
                }
                gridArray[x, y] = createGridObject(this, x, y, m);
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        gridArray[x, y] = value;
    }

    public TGridObject GetValue(int x, int y)
    {
        return gridArray[x, y];
    }
}
