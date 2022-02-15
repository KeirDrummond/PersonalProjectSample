using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap : MonoBehaviour
{
    public Tilemap map;
    public Tilemap blueMap;
    public Tile blueTile;
    public Tile redTile;
    public Camera cam;
    public GameObject root;
    public MapCanvas mapCanvas;
    public MapPlayerController playerController;
    public Transform mapObjects;
    public Indicator indicator;

    private void Awake()
    {
        GameManager.Instance.GetMapManager().SetupMap(map, blueMap, blueTile, redTile, cam, mapCanvas, playerController, root, mapObjects, indicator);
    }
}
