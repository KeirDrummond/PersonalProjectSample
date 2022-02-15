using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.IsPaused())
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        //pauseMenuUI.SetActive(true);
        //GameManager.Instance.PauseGame();
    }

    private void Resume()
    {
        //pauseMenuUI.SetActive(false);
        //GameManager.Instance.UnpauseGame();
    }
}
