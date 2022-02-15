using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.player.GetMapPlayerController().AttackCommand();
    }
}
