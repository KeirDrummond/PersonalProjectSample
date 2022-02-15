using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.player.GetMapPlayerController().WaitCommand();
    }
}
