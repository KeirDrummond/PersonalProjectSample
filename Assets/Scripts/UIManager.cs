using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject loseScreen;

    public void WinScreen()
    {
        winScreen.SetActive(true);
    }

    public void LoseScreen()
    {
        loseScreen.SetActive(true);
    }
}
