using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Controller : MonoBehaviour
{
    // Handles inputs either from a human player or an AI controlled player.

    [SerializeField]
    protected Team team;

    protected bool myTurn;

    public abstract void StartOfTurn();

    public abstract void EndOfTurn();

}
