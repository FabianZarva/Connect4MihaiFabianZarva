using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Load a Player vs Player Match (First Round)
    public void PlayPVPGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    // Load a Player vs Computer (AI) Match (First Round)
    public void PlayPVAIGame()
    {
        SceneManager.LoadScene(10);
    }

    // Quit the Game
    public void QuitGame()
    {
        Application.Quit();
    }
}
