using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager
{
    private GameObject root;
    private Transform mapObjects;
    private Camera cam;

    public Tilemap tilemap;
    public MapUnit player;

    public Tile blueTile;
    public Tile redTile;
    public Tilemap blueTilemap;

    public Indicator indicator;

    private MapPathing pathing;

    bool isSelected = false;

    List<MapUnit> enemies;

    private MapCanvas mapCanvas;

    private MapPlayerController playerController;

    public MapManager()
    {
    }

    public void SetupMap(Tilemap map, Tilemap blueMap, Tile blueTile, Tile redTile, Camera cam, MapCanvas mapCanvas, MapPlayerController pc, GameObject root, Transform mapObjects, Indicator indicator)
    {
        tilemap = map;
        blueTilemap = blueMap;
        this.blueTile = blueTile;
        this.redTile = redTile;
        tilemap.CompressBounds();
        this.cam = cam;
        this.mapCanvas = mapCanvas;
        playerController = pc;
        this.root = root;
        this.mapObjects = mapObjects;
        this.indicator = indicator;

        pathing = new MapPathing(tilemap);
        Debug.Log(root);
        GameManager.Instance.SetupCharacters(mapObjects);

        MapUnit[] units = GameObject.FindObjectsOfType<MapUnit>();
        enemies = new List<MapUnit>();
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].tag == "Enemy")
            {
                enemies.Add(units[i]);
            }
        }
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public MapPathing GetPathing()
    {
        return pathing;
    }

    public MapCanvas GetCanvas()
    {
        return mapCanvas;
    }

    public MapPlayerController GetMapPlayerController()
    {
        return playerController;
    }

    public void StartBattle(Vector3Int location, ref GameCharacter player, ref GameCharacter enemy)
    {
        MapData map = tilemap.GetTile<MapTile>(location).map;
        BattleData data = new BattleData(ref player, ref enemy, map);
        GameManager.Instance.StartBattle(data);

        root.SetActive(false);
        cam.gameObject.SetActive(false);
    }

    public void EndBattle(BattleData data, BattleResults results)
    {
        root.SetActive(true);
        cam.gameObject.SetActive(true);

        if (data.attacker == results.winner)
        {
            if (!results.retreated)
            {
                UnitDefeated(data.defender);
            }
            else
            {
                Vector2Int aPosition = data.attacker.mapUnit.GetPosition();
                Vector2Int dPosition = data.defender.mapUnit.GetPosition();

                data.attacker.mapUnit.SetPosition(dPosition);
                data.defender.mapUnit.SetPosition(aPosition);
            }
        }
        else if (data.defender == results.winner)
        {
            if (!results.retreated)
            {
                UnitDefeated(data.attacker);
            }
        }
    }

    private void UnitDefeated(GameCharacter unit)
    {
        unit.active = false;
        Debug.Log(unit.mapUnit.gameObject);
        unit.mapUnit.gameObject.SetActive(false);

        if (IsMapOver()) { SceneManager.LoadScene("TitleScene"); }
    }

    private bool IsMapOver()
    {
        List<GameCharacter> units = GameManager.Instance.GetCharacterList();

        int remainingUnits(Team team)
        {
            int count = 0;
            foreach (GameCharacter unit in units)
            {
                if (unit.team == team)
                {
                    if (unit.active) { count++; }
                }
            }
            return count;
        }

        if (remainingUnits(Team.PLAYER) == 0) { return true; }
        else if (remainingUnits(Team.ENEMY) == 0) { return true; }
        return false;
    }

    public GameCharacter GetUnitAtPosition(Vector2Int position)
    {
        List<GameCharacter> units = GameManager.Instance.GetCharacterList();
        for (int i = 0; i < units.Count; i++)
        {
            GameCharacter unit = units[i];
            if (!unit.active) { continue; }
            if (unit.mapUnit.GetPosition() == position) { return units[i]; }
        }
        return null;
    }

    public void WaitAtPosition(MapUnit unit, Vector2Int position)
    {
        unit.SetPosition(position);
    }

    public List<Vector2Int> GetNeighbouringTiles(Vector2Int position, int range)
    {
        List<Vector2Int> neighbourList = new List<Vector2Int>();
        neighbourList.Add(position);

        for (int i = 1; i <= range; i++)
        {
            List<Vector2Int> additions = new List<Vector2Int>();
            foreach (Vector2Int t in neighbourList)
            {
                List<Vector2Int> n = GetNeighbors(t);
                foreach (Vector2Int tile in n)
                {
                    if (!neighbourList.Contains(tile))
                    {
                        additions.Add(tile);
                    }
                }
            }
            foreach (Vector2Int a in additions)
            {
                neighbourList.Add(a);
            }
        }

        neighbourList.RemoveAt(0);

        List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> n = new List<Vector2Int>();

            Vector2Int[] evenNeighbours =
                {
            new Vector2Int(1, 0),   new Vector2Int(0, 1),    new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),  new Vector2Int(-1, -1),   new Vector2Int(0, -1)
            };

            Vector2Int[] oddNeighbours =
                {
            new Vector2Int(1, 0),   new Vector2Int(1, 1),  new Vector2Int(0, 1),
            new Vector2Int(-1, 0),  new Vector2Int(0, -1),   new Vector2Int(1, -1)
            };

            bool IsValid(Vector2Int position)
            {
                if (position.x < 0 || position.x >= tilemap.size.x) return false;
                if (position.y < 0 || position.y >= tilemap.size.y) return false;
                return true;
            }

            if (position.y % 2 == 0)
            {
                foreach (Vector2Int vector in evenNeighbours)
                {
                    Vector2Int neighbour = position + vector;
                    if (IsValid(neighbour)) n.Add(neighbour);
                }
            }
            else
            {
                foreach (Vector2Int vector in oddNeighbours)
                {
                    Vector2Int neighbour = position + vector;
                    if (IsValid(neighbour)) n.Add(neighbour);
                }
            }

            return n;
        }

        return neighbourList;
    }

}