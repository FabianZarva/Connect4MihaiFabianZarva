using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AIGameManager : MonoBehaviour
{
    // Player pieces
    public GameObject player1;
    public GameObject player2;

    // Ghost pieces to visualize dropping position
    public GameObject player1Ghost;
    public GameObject player2Ghost;

    // Canvases for displaying who wins and draws 
    public GameObject winCanvas;
    public GameObject winCanvas2;
    public GameObject drawCanvas;
    
    // Text component for displaying the timer
    public Text timerText;

    // Turn time and timer variables
    public float turnTime = 15f;
    private float timer;

    // Dimensions of the game board
    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject[] spawnLoc; // Spawn locations for pieces

    // The falling piece
    GameObject fallingPiece;

    // Tracking the game state
    bool player1Turn = true;
    bool playerWon = false;
    bool gamePaused = false; // Track whether the game is paused or not

    // Tracking the board state
    int[,] boardState; // 0 = empty, 1 = player 1, 2 = player 2

    // Start is called before the first frame update
    void Start()
    {
        // Initializing the board state and hiding ghost pieces from the game scene view
        boardState = new int[lengthOfBoard, heightOfBoard];
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        StartTurnTimer(); // Start the turn timer for the first player

        // If it's player 2's turn, simulate AI move
        if (!player1Turn)
        {
            int aiColumn = AISelectColumn();
            StartCoroutine(AIDelay(aiColumn));
        }
    }

    void Update()
    {
        if (!gamePaused && !playerWon)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SkipTurn();
            }
            UpdateTimerText(); // Update the timer text on the screen
        }
    }

    // Start the timer for the current player's turn
    void StartTurnTimer()
    {
        timer = turnTime;
        UpdateTimerText(); // Update the timer text on the screen when starting the timer
    }

    // Update the timer text on the screen
    void UpdateTimerText()
    {
        timerText.text = Mathf.Ceil(timer).ToString(); // Display the remaining time as an integer
    }

    // Hovering with the mouse over columns
    public void HoverColumn(int column)
    {
        // Display the ghost piece on valid columns if the game is not paused, showcasing where the player can choose the piece to fall
        if (!gamePaused && boardState[column, heightOfBoard - 1] == 0 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero) && !playerWon)
        {
            if (player1Turn)
            {
                player1Ghost.SetActive(true);
                player1Ghost.transform.position = spawnLoc[column].transform.position;
            }
            else
            {
                player2Ghost.SetActive(true);
                player2Ghost.transform.position = spawnLoc[column].transform.position;
            }
        }
    }

    // Selecting a column to drop a piece
    public void SelectColumn(int column)
    {
        // Check if the game is not paused, if the game is not won by someone or drawn, and if it's a valid move 
        if (!gamePaused && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero) && !playerWon)
        {
            TakeTurn(column);
        }
    }

    // Players taking turns
    void TakeTurn(int column)
    {
        if (UpdateBoardState(column))
        {
            player1Ghost.SetActive(false);
            player2Ghost.SetActive(false);

            if (player1Turn)
            {
                fallingPiece = Instantiate(player1, spawnLoc[column].transform.position, Quaternion.identity);
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
                player1Turn = false;
                fallingPiece.transform.Rotate(Vector3.up, 90f);

                if (Didwin(1))
                {   
                    playerWon = true;
                    winCanvas.SetActive(true);
                }
            }
            else
            {
                fallingPiece = Instantiate(player2, spawnLoc[column].transform.position, Quaternion.identity);
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
                player1Turn = true;
                fallingPiece.transform.Rotate(Vector3.up, 90f);

                if (Didwin(2))
                {
                    playerWon = true;
                    winCanvas2.SetActive(true);
                }
            }

            if (DidDraw())
            {
                playerWon = true; 
                drawCanvas.SetActive(true);
            }

            StartTurnTimer(); // Restart the turn timer after making a move
            
            // If it's AI's turn, simulate AI move
            if (!player1Turn && !playerWon)
            {
                int aiColumn = AISelectColumn();
                StartCoroutine(AIDelay(aiColumn));
            }
        }
    }

    // Coroutine for delaying AI move
    IEnumerator AIDelay(int column)
    {
        yield return new WaitForSeconds(1f); // Delay the AI move to make it look more natural
        TakeTurn(column);
    }

    // AI selects a column to drop a piece
    int AISelectColumn()
    {
        // Simple AI: randomly select a valid column
        List<int> validColumns = new List<int>();
        for (int i = 0; i < lengthOfBoard; i++)
        {
            if (boardState[i, heightOfBoard - 1] == 0)
            {
                validColumns.Add(i);
            }
        }
        if (validColumns.Count > 0)
        {
            int randomIndex = Random.Range(0, validColumns.Count);
            return validColumns[randomIndex];
        }
        else
        {
            Debug.LogError("No valid columns available for AI move!");
            return -1;
        }
    }

    // Update the board state after a piece is dropped
    bool UpdateBoardState(int column)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                if (player1Turn)
                {
                    boardState[column, row] = 1;
                }
                else
                {
                    boardState[column, row] = 2;
                }
                return true;
            }
        }
        Debug.LogWarning("Column " + column + " is full");
        return false;
    }

    // Check if one of the players wins
    bool Didwin(int playerNum)
    {
        // Check horizontally
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x + 1, y] == playerNum && boardState[x + 2, y] == playerNum && boardState[x + 3, y] == playerNum)
                {
                    return true;
                }
            }
        }

        // Check vertically
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x, y + 1] == playerNum && boardState[x, y + 2] == playerNum && boardState[x, y + 3] == playerNum)
                {
                    return true;
                }
            }
        }

        // Check diagonally to the right
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x + 1, y + 1] == playerNum && boardState[x + 2, y + 2] == playerNum && boardState[x + 3, y + 3] == playerNum)
                {
                    return true;
                }
            }
        }

        // Check diagonally to the left
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 3; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x + 1, y - 1] == playerNum && boardState[x + 2, y - 2] == playerNum && boardState[x + 3, y - 3] == playerNum)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Check if the game ends in a draw
    bool DidDraw()
    {
        for (int x = 0; x < lengthOfBoard; x++)
        {
            if (boardState[x, heightOfBoard - 1] == 0)
            {
                return false;
            }
        }
        return true;
    }

    void SkipTurn()
    {
    player1Turn = !player1Turn; // Toggle the player turn
    StartTurnTimer(); // Restart the turn timer

    // If it's now player 2's turn, simulate AI move
    if (!player1Turn && !playerWon)
      {
        int aiColumn = AISelectColumn();
        StartCoroutine(AIDelay(aiColumn));
      }
    }

    // Set the game paused or unpaused
    public void SetGamePaused(bool paused)
    {
        gamePaused = paused;
        if (paused)
        {
            Time.timeScale = 0; // Pause the game by setting the time scale to 0
        }
        else
        {
            Time.timeScale = 1; // Resume the game by setting the time scale back to 1
        }
    }

    // Restart the game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Advance to different levels (if applicable)
    public void AdvanceLevel01()
    {
        SceneManager.LoadScene(11);
    }

    public void AdvanceLevel02()
    {
        SceneManager.LoadScene(12);
    }
    
    public void AdvanceLevel10()
    {
        SceneManager.LoadScene(13);
    }
    public void AdvanceLevel11()
    {
        SceneManager.LoadScene(14);
    }
    public void AdvanceLevel12()
    {
        SceneManager.LoadScene(15);
    }
   
    public void AdvanceLevel20()
    {
        SceneManager.LoadScene(16);
    }
    public void AdvanceLevel21()
    {
        SceneManager.LoadScene(17);
    }
    public void AdvanceLevel22()
    {
        SceneManager.LoadScene(18);
    }
}
