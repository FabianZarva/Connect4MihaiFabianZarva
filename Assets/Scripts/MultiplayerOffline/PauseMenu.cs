using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu; // Reference to the pause menu GameObject within the level canvas
    public GameManager gameManager; // Reference to the GameManager script

    // Pausing the game
    public void Pause()
    {
        pauseMenu.SetActive(true); // Activating the pause menu UI
        gameManager.SetGamePaused(true); // Setting the game as paused through the GameManager - the game can t be played further until it is resumed
        Time.timeScale = 0; // Setting the time scale to 0 to pause the game, basically stopping the game flow
    }

    // Returning to the main menu
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu"); // Loading the main menu scene
    }

    // Resuming the game
    public void Resume()
    {
        pauseMenu.SetActive(false); // Deactivating the pause menu UI
        gameManager.SetGamePaused(false); // Setting the game as unpaused through the GameManager - the game can be played from where it was left off
        Time.timeScale = 1; // Set the time scale back to 1 to resume the game flow
    }

    // Restarting the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloading the current scene to restart the game - giving the players another round
    }
}
