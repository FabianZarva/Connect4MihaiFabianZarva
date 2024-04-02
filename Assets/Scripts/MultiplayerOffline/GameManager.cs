using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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

     public Text timerText; // Reference to the Text component displaying the timer
    public float turnTime = 15f; // Time allocated for each player's turn
    private float timer; // Timer for tracking turn time

    

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

    //Tracking the board state
    int[,] boardState; // 0 = empty , 1 = player 1 , 2 = player 2

    // Start is called before the first frame update
    void Start()
    {
        // Initializing the board state and hiding ghost pieces from the game scene view
        boardState = new int[lengthOfBoard, heightOfBoard];
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
         StartTurnTimer(); // Start the turn timer for the first player
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

      void UpdateTimerText()
    {
        timerText.text = Mathf.Ceil(timer).ToString(); // Display the remaining time as an integer
    }

    // Hovering with the mouse over columns
    public void HoverColumn(int column)
    {
        // Display the ghost piece on valid columns if game is not paused, showcasing where can the player choose the piece to fall
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
        // Checking if game is not paused , if the game is not won by someone or drawn and if it's a valid move 
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
        }
    }

     void SkipTurn()
    {
        if (player1Turn)
        {
            Debug.Log("Player 1's turn has been skipped!");
        }
        else
        {
            Debug.Log("Player 2's turn has been skipped!");
        }
        player1Turn = !player1Turn; // Switch to the other player's turn
        StartTurnTimer(); // Restart the turn timer for the next player
    }

    // The board is updated after a piece is dropped
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
        Debug.LogWarning("Column" + column + "isFull");
        return false;
    }

    // Did one of the players win?
    bool Didwin(int playerNum)
    {
        // Are there 4 consecutive same-color pieces horizontally?
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

        // Are there 4 consecutive same-color pieces vertically?
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

        // Are there 4 consecutive same-color pieces diagonally to the right?
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

        //Are there 4 consecutive same-color pieces diagonally to the left?
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

    // Did the game end in a draw?
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

    // Setting the game paused or unpaused(still in play)
    public void SetGamePaused(bool paused)
    {
        gamePaused = paused;
    }

    // Restarting the game
    public void RestartMatch()
    {
        SceneManager.LoadScene(1);
    }

    public void RestartRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  
    //Loading the Main Menu
   public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Quitting the game
    public void QuitGame()
    {
        Application.Quit();
    }

    public void AdvanceLevel01()
    {
        SceneManager.LoadScene(2);
    }

    public void AdvanceLevel02()
    {
        SceneManager.LoadScene(3);
    }
    
    public void AdvanceLevel10()
    {
        SceneManager.LoadScene(4);
    }
    public void AdvanceLevel11()
    {
        SceneManager.LoadScene(5);
    }
    public void AdvanceLevel12()
    {
        SceneManager.LoadScene(6);
    }
   
    public void AdvanceLevel20()
    {
        SceneManager.LoadScene(7);
    }
    public void AdvanceLevel21()
    {
        SceneManager.LoadScene(8);
    }
    public void AdvanceLevel22()
    {
        SceneManager.LoadScene(9);
    }
   
}
