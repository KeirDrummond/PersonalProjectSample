using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MapObject
{
    public void ShiftPosition(Vector2Int direction)
    {
        Vector2Int newPosition = position + direction;
        if (!(MapTile)tilemap.GetTile((Vector3Int)newPosition)) { return; }
        SetPosition(newPosition);
        Vector3 pos = tilemap.GetCellCenterWorld((Vector3Int)position);
        transform.position = new Vector3(pos.x, pos.y);
    }

    public void MoveToLocation(Vector3 location)
    {
        Vector3Int cell = tilemap.WorldToCell(location);
        cell.z = 0;
        if (!(MapTile)tilemap.GetTile(cell)) { return; }
        Vector2Int newPosition = (Vector2Int)(cell - tilemap.origin);
        SetPosition(newPosition);
        Vector3 pos = tilemap.GetCellCenterWorld(cell);
        transform.position = new Vector3(pos.x, pos.y);
    }
}
