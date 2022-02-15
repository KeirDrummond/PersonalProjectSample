using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class MapUnit : MapObject
{
    private GameCharacter unit;
    private SpriteRenderer spriteRenderer;

    MapPathing pathing;

    private bool selected;
    private bool acted;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();        

        pathing = mapManager.GetPathing();

        acted = false;
        selected = false;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null) { Debug.Log("No Sprite"); return; }
        spriteRenderer.sprite = sprite;
    }

    public GameCharacter GetGameCharacter()
    {
        return unit;
    }

    public void NewTurn()
    {
        acted = false;
    }

    public void SetVisualPosition(Vector2Int position)
    {
        transform.position = tilemap.GetCellCenterWorld((Vector3Int)position);
    }

    public void SelectUnit()
    {
        selected = true;
    }

    public void DeselectUnit()
    {
        selected = false;
        SetVisualPosition(position);
    }

    public void Wait(Vector2Int position)
    {
        SetPosition(position);
    }

    public bool HasUnitedActed() { return acted; }

    public void EndOfAction()
    {
        DeselectUnit();
        acted = true;
    }

}
