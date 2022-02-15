using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MapPlayerController : MapController
{
    // All of the player inputs on the map scene is managed in this class.

    private MapManager mapManager;

    private Indicator indicator; // The indicator follows the cursor around.

    private GameCharacter selectedUnit;
    private Vector2Int selectedPosition;
    private GameCharacter selectedTarget;
    private MapCanvas canvas;

    public MapPlayerController()
    {
        mapManager = GameManager.Instance.GetMapManager();
        indicator = mapManager.indicator;
        canvas = mapManager.GetCanvas();
    }

    public void Update()
    {
        Input();
    }

    private void Input()
    {
        if (indicator == null) { indicator = mapManager.indicator; }
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            indicator.ShiftPosition(Vector2Int.up);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            indicator.ShiftPosition(Vector2Int.down);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
            indicator.ShiftPosition(Vector2Int.left);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            indicator.ShiftPosition(Vector2Int.right);
        }
        if (UnityEngine.Input.GetAxis("Mouse X") != 0 || UnityEngine.Input.GetAxis("Mouse Y") != 0)
        {
            indicator.MoveToLocation(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectTile();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (canvas.menuOpen)
            {
                canvas.CloseMenu();
            }
            else if (selectedUnit != null)
            {
                DeselectUnit();
            }
        }
    }

    private void EndAction()
    {
        // If all my dudes have acted, end turn.
        List<GameCharacter> myUnits = GetMyUnits();
        foreach(GameCharacter unit in myUnits)
        {
            if (unit.active && unit.mapUnit.HasUnitedActed()) {
                EndTurn();
                break; 
            }
        }
    }

    private List<GameCharacter> GetMyUnits()
    {
        List<GameCharacter> list = GameManager.Instance.GetCharacterList();
        List<GameCharacter> myUnits = new List<GameCharacter>();

        foreach(GameCharacter unit in list)
        {
            if (unit.team == team)
            {
                myUnits.Add(unit);
            }
        }

        return myUnits;
    }

    private void SelectUnit(GameCharacter unit)
    {
        if (unit.team != team) { return; }
        unit.mapUnit.SelectUnit();
        selectedUnit = unit;
        ShowPossibleMovement(unit, unit.mapUnit.GetPosition());
    }

    private void DeselectUnit()
    {
        selectedUnit.mapUnit.DeselectUnit();
        selectedUnit = null;
        GameManager.Instance.GetMapManager().blueTilemap.ClearAllTiles();
    }

    private void ShowPossibleMovement(GameCharacter unit, Vector2Int position)
    {
        MapManager mapManager = GameManager.Instance.GetMapManager();

        int movement = unit.movement;
        MapPathing pathing = mapManager.GetPathing();
        List<Vector2Int> tiles = pathing.GetMovementRange(position.x, position.y, movement);

        Tilemap blueMap = mapManager.blueTilemap;
        Tile blueTile = mapManager.blueTile;
        Tile redTile = mapManager.redTile;

        foreach (Vector2Int tile in tiles)
        {
            Vector3Int tilePosition = new Vector3Int(tile.x, tile.y);

            if (IsSpaceFree(unit, (Vector2Int)tilePosition))
            {
                blueMap.SetTile(tilePosition, blueTile);
            }
            else
            {
                blueMap.SetTile(tilePosition, redTile);
            }

            blueMap.CompressBounds();
        }
    }

    private List<GameCharacter> TargetsInRange(GameCharacter unit, Vector2Int position, int range)
    {
        MapPathing pathing = mapManager.GetPathing();
        List<GameCharacter> units = new List<GameCharacter>();

        List<Vector2Int> tiles = mapManager.GetNeighbouringTiles(position, range);
        foreach (Vector2Int tile in tiles)
        {
            GameCharacter otherUnit = mapManager.GetUnitAtPosition(tile);
            List<MapPathNode> path = pathing.FindPath(position.x, position.y, tile.x, tile.y);
            if (otherUnit != null && otherUnit.team != team && path != null && path[path.Count - 1].gCost < unit.movement)
            {
                units.Add(otherUnit);
            }
        }

        return units;
    }

    private bool IsSpaceFree(GameCharacter myUnit, Vector2Int position)
    {
        GameCharacter unit = mapManager.GetUnitAtPosition(position);
        if (unit == null) { return true; }
        else if (unit == myUnit) { return true; }
        return false;
    }

    private bool CanMoveTo(GameCharacter unit, Vector2Int destination)
    {
        MapPathing pathing = mapManager.GetPathing();
        List<MapPathNode> path = pathing.FindPath(unit.mapUnit.GetPosition().x, unit.mapUnit.GetPosition().y, destination.x, destination.y);
        if (path != null && IsSpaceFree(unit, destination) && path[path.Count - 1].gCost <= unit.movement)
        {
            return true;
        }
        else { return false; }
    }

    private void SelectTile()
    {
        Vector2Int pos = indicator.GetPosition();
        GameCharacter unit = mapManager.GetUnitAtPosition(pos);
        if (unit != null && unit.mapUnit.tag == "Player" && selectedUnit == null && !canvas.menuOpen)
        {
            SelectUnit(unit);
        }
        else if (selectedUnit != null && !canvas.menuOpen)
        {
            if (CanMoveTo(selectedUnit, pos))
            {
                selectedUnit.mapUnit.SetVisualPosition(pos);
                selectedPosition = pos;

                List<Button> buttons = GenerateButtonList(pos);

                Tilemap tilemap = mapManager.GetTilemap();
                Vector3 location = tilemap.GetCellCenterWorld((Vector3Int)pos);

                canvas.OpenMenu(buttons, location);
            }
        }
    }

    private List<Button> GenerateButtonList(Vector2Int location)
    {
        List<Button> buttons = new List<Button>();
        buttons.Add(canvas.waitButton);

        if (TargetsInRange(selectedUnit, location, 1).Count >= 1)
        {
            buttons.Add(canvas.attackButton);
        }

        return buttons;
    }

    public void WaitCommand()
    {
        canvas.CloseMenu();
        WaitAtLocation(selectedUnit, selectedPosition);        
        DeselectUnit();

        EndAction();
    }

    public void AttackCommand()
    {
        canvas.CloseMenu();
        List<GameCharacter> units = TargetsInRange(selectedUnit, selectedPosition, 1);
        GameCharacter unit = units[0];
        GameCharacter myUnit = selectedUnit;
        selectedTarget = unit;
        DeselectUnit();
        AttackTarget(myUnit, selectedTarget, selectedPosition);        

        EndAction();
    }
}