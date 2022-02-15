using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    MapAIController mapController;

    // Start is called before the first frame update
    void Start()
    {
        mapController = new MapAIController(team);
    }

    public override void StartOfTurn()
    {
        myTurn = true;
        mapController.StartOfTurn();
        StartCoroutine(MyTurn());
    }

    public override void EndOfTurn()
    {
        myTurn = false;
        StopCoroutine(MyTurn());
    }

    private IEnumerator MyTurn()
    {
        while (myTurn)
        {
            if (GameManager.Instance.battleState == BattleState.BATTLE)
            {
                yield return new WaitUntil(() => GameManager.Instance.battleState == BattleState.MAP);
            }

            mapController.MyTurn();
            yield return new WaitForSeconds(1f);
        }
        
    }
}
