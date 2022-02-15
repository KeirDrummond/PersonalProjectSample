using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapObject : MonoBehaviour
{
    protected Vector2Int position;
    protected Tilemap tilemap;
    protected MapManager mapManager;

    virtual protected void Start()
    {
        mapManager = GameManager.Instance.GetMapManager();
        tilemap = mapManager.GetTilemap();
        transform.position = GetWorldPosition();
    }

    public void SetPosition(Vector2Int position)
    {
        if (tilemap == null) { tilemap = GameManager.Instance.GetMapManager().GetTilemap(); }
        if (IsPositionValid(position))
        {
            this.position = position;
            transform.position = GetWorldPosition();
        }
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public Vector3 GetWorldPosition()
    {
        Vector3Int tilemapPosition = tilemap.origin + (Vector3Int)position;
        return tilemap.GetCellCenterWorld(tilemapPosition);
    }

    protected bool IsPositionValid(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0) return false;
        if (position.x > tilemap.size.x || position.y > tilemap.size.y) return false;
        
        return true;
    }

}
