using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameCamera : MonoBehaviour
{
    public float minX, minY;
    public float maxX, maxY;

    public BattleUnit character;
    public Tilemap tilemap;

    public void SetBounds(int xMinBound, int xMaxBound)
    {
        tilemap.CompressBounds();
        Grid grid = tilemap.layoutGrid;
        float cellWidth = grid.cellSize.x;
        float cellHeight = grid.cellSize.y;
        Vector3Int size = tilemap.size;

        float battleMinX = tilemap.GetCellCenterWorld(new Vector3Int(xMinBound, 0)).x - (cellWidth / 2);
        minX = tilemap.GetCellCenterWorld(tilemap.origin).x - (cellWidth / 2);
        minX = Mathf.Max(minX, battleMinX);
        Debug.Log(cellWidth);

        float battleMaxX = tilemap.GetCellCenterWorld(new Vector3Int(xMaxBound, 0)).x + (cellWidth / 2);
        maxX = tilemap.GetCellCenterWorld(tilemap.origin + size).x - (cellWidth / 2);
        maxX = Mathf.Min(maxX, battleMaxX);

        minY = tilemap.GetCellCenterWorld(tilemap.origin).y - (cellHeight / 2);

        maxY = tilemap.GetCellCenterWorld(tilemap.origin + size).y - (cellHeight / 2);
    }

    public void FollowCharacter(BattleUnit character)
    {
        this.character = character;
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null && tilemap != null)
        {
            Camera camera = GetComponent<Camera>();
            float camHeight = camera.orthographicSize * 2;
            float camWidth = camHeight * camera.aspect;

            float posX = character.transform.position.x;
            posX = Mathf.Min(posX, maxX - camWidth / 2);
            posX = Mathf.Max(posX, minX + camWidth / 2);

            float posY = character.transform.position.y;
            posY = Mathf.Min(posY, maxY - camHeight / 2);
            posY = Mathf.Max(posY, minY + camHeight / 2);

            gameObject.transform.position = new Vector3(posX, posY, -10);
        }
    }
}
