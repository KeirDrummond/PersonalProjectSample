using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager
{
    private GameObject root;
    private Canvas canvas;

    private HealthBar playerHealthBar;

    private Tilemap collision;
    private Tilemap platforms;
    private GameCamera gameCamera;

    private BattleData currentBattle;

    private BattleUnit attacker;
    private BattleUnit defender;

    public BattleManager()
    {
    }

    public HealthBar GetPlayerHealthBar() { return playerHealthBar; }

    public void SetupBattleScene(Tilemap collision, Tilemap platforms, Canvas canvas, HealthBar playerHealthBar, GameCamera cam, GameObject root)
    {
        this.collision = collision;
        this.platforms = platforms;
        this.canvas = canvas;
        this.playerHealthBar = playerHealthBar;
        this.root = root;
        this.gameCamera = cam;
        canvas.gameObject.SetActive(false);
        gameCamera.gameObject.SetActive(false);
    }

    public void NewBattle(BattleData data)
    {
        currentBattle = data;

        PopulateTilemap(collision, data.map.collision);
        PopulateTilemap(platforms, data.map.platforms);
        LoadAStar(data.map.AStarGraph);
        attacker = SpawnCharacter(data.attacker, data.map.attackerSpawn, data.attacker.team);
        defender = SpawnCharacter(data.defender, data.map.defenderSpawn, data.defender.team);
        gameCamera.gameObject.SetActive(true);
        gameCamera.SetBounds(data.map.minXBound, data.map.maxXBound);

        canvas.gameObject.SetActive(true);
        playerHealthBar.transform.GetChild(0).localScale = Vector3.one;
    }

    public void UnitRetreated()
    {
        if (IsBattleOver()) EndBattle();
    }

    public void UnitDefeated()
    {
        if (IsBattleOver()) EndBattle();
    }

    private bool IsBattleOver()
    {
        if (attacker.defeated || attacker.retreated || defender.defeated || defender.defeated) { return true; }
        return false;
    }

    public float GetRetreatLine()
    {
        return currentBattle.map.minXBound;
    }

    private void PopulateTilemap(Tilemap tilemap, Tilemap newMap)
    {
        Vector3Int origin = newMap.origin;
        Vector3Int end = newMap.origin + newMap.size;
        for (int x = origin.x; x <= end.x; x++)
        {
            for (int y = origin.y; y <= end.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y);
                TileBase tile = newMap.GetTile(position);
                tilemap.SetTile(position, tile);
            }
        }
        tilemap.CompressBounds();
    }

    private void LoadAStar(TextAsset AStarGraph)
    {
        AstarPath.active.data.DeserializeGraphs(AStarGraph.bytes);
    }

    private BattleUnit SpawnCharacter(GameCharacter character, Vector3 position, Team team)
    {
        BattleUnit unit = character.battleUnit;
        unit = Object.Instantiate(unit, position, Quaternion.identity, root.transform);
        unit.team = team;

        if (team == Team.PLAYER)
        {
            BattlePlayerController playerController = GameManager.Instance.player.GetBattlePlayerController();
            playerController.ControlCharacter(unit);
            gameCamera.FollowCharacter(unit);
        }

        return unit;
    }

    private void EndBattle()
    {
        if (root.transform.childCount != 0) { 
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Object.Destroy(root.transform.GetChild(i).gameObject);
            }
        }
        canvas.gameObject.SetActive(false);
        collision.ClearAllTiles();
        platforms.ClearAllTiles();
        gameCamera.gameObject.SetActive(false);

        GameCharacter winner = currentBattle.defender;
        bool retreat = false;

        if (attacker.retreated) { winner = currentBattle.defender; retreat = true; }
        else if (attacker.defeated) { winner = currentBattle.defender; retreat = false; }
        else if (defender.retreated) { winner = currentBattle.attacker; retreat = true; }
        else if (defender.defeated) { winner = currentBattle.attacker; retreat = false; }

        if (attacker.retreated || attacker.retreated)
        {
            winner = currentBattle.defender;
        }
        else if (defender.retreated || defender.defeated)
        {
            winner = currentBattle.attacker;
        }

        BattleData data = currentBattle;
        currentBattle = null;
        BattleResults results = new BattleResults(winner, retreat);
        GameManager.Instance.EndBattle(data, results);
    }
}
