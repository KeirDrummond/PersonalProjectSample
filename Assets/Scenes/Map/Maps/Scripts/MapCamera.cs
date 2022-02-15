using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCamera : MonoBehaviour
{
    float minX, minY;
    float maxX, maxY;
    public Indicator indicator;
    public Tilemap tilemap;

    public int edgeSize;
    public float scrollSpeed;

    Rect rect;

    public float cellWidth;
    public float cellHeight;

    // Start is called before the first frame update
    void Start()
    {
        tilemap.CompressBounds();
        Vector3Int size = tilemap.size;
        float e = size.x % 2 == 0 ? cellWidth / 2 : 0;

        minX = tilemap.GetCellCenterWorld(tilemap.origin).x - cellWidth / 2;        
        minY = tilemap.GetCellCenterWorld(tilemap.origin).y - cellHeight / 2;
        maxX = tilemap.GetCellCenterWorld(tilemap.origin + size - Vector3Int.one).x + cellWidth / 2 + e;
        maxY = tilemap.GetCellCenterWorld(tilemap.origin + size - Vector3Int.one).y + cellHeight / 2;

        Camera camera = GetComponent<Camera>();

        camera.orthographicSize = 6;
        camera.aspect = 2;
        float camHeight = camera.orthographicSize * 2;
        float camWidth = camHeight * camera.aspect;

        rect = new Rect(-camWidth / 2, -camHeight / 2, camWidth, camHeight);

        Vector3 position = new Vector3(indicator.GetWorldPosition().x, indicator.GetWorldPosition().y, transform.position.z);
        position.x = Mathf.Clamp(position.x, minX + camWidth / 2, maxX - camWidth / 2);
        position.y = Mathf.Clamp(position.y, minY + camHeight / 2, maxY - camHeight / 2);
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        bool menuOpen = GameManager.Instance.GetMapManager().GetCanvas().menuOpen;
        if (!menuOpen)
        {
            MoveCamera();
        }        
    }

    void MoveCamera()
    {
        if (indicator.transform.position.x < rect.xMin + edgeSize) { rect.position += Vector2.left; }
        else if (indicator.transform.position.x > rect.xMax - edgeSize) { rect.position += Vector2.right; }
        if (indicator.transform.position.y > rect.yMax - edgeSize) { rect.position += Vector2.up; }
        else if (indicator.transform.position.y < rect.yMin + edgeSize) { rect.position += Vector2.down; }

        rect.x = Mathf.Clamp(rect.x, minX, maxX - rect.width);
        rect.y = Mathf.Clamp(rect.y, minY, maxY - rect.height);

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(rect.center.x, rect.center.y, transform.position.z), scrollSpeed * Time.deltaTime);
    }

}