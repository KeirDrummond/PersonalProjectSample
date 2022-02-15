using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    MapPlayerController mapController;
    BattlePlayerController battleController;

    private void Start()
    {
        mapController = new MapPlayerController();
        battleController = new BattlePlayerController();
    }

    // Which control scheme should be read?
    void Update()
    {
        GameState gameState = GameManager.Instance.gameState;
        BattleState battleState = GameManager.Instance.battleState;

        if (battleState == BattleState.BATTLE) { battleController.Update(); }
        else if (battleState == BattleState.MAP && gameState == GameState.PLAYERTURN) { mapController.Update(); }
    }
    public override void StartOfTurn()
    {
        myTurn = true;
    }

    public override void EndOfTurn()
    {
        myTurn = false;
    }

    public MapPlayerController GetMapPlayerController()
    {
        return mapController;
    }

    public BattlePlayerController GetBattlePlayerController()
    {
        return battleController;
    }
}
