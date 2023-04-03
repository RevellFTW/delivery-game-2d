using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    public GameObject UI;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        UI.SetActive(false);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    void Resume()
    {
        PauseMenuUI.SetActive(false);
        UI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
