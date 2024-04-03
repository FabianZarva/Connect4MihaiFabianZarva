using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayPVPGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayPVAIGame()
    {
        SceneManager.LoadScene(10);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
