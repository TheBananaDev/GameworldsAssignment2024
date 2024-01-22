using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNav : MonoBehaviour
{
    public GameObject ControlsPanel;

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ControlsButton(bool onPage)
    {
        ControlsPanel.SetActive(onPage);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void UpdateMouseSensitivity(float scale)
    {
        Player.instance.UpdateSensitivity(scale);
    }
}
