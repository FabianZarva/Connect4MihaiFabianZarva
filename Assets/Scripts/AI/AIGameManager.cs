using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AIGameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public GameObject player1Ghost;
    public GameObject player2Ghost;

    public GameObject winCanvas;
    public GameObject winCanvas2;
    public GameObject drawCanvas;
    
    public Text timerText;

    public float turnTime = 15f;
    private float timer;

    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject[] spawnLoc;

    GameObject fallingPiece;

    bool player1Turn = true;
    bool playerWon = false;
    bool gamePaused = false;

    int[,] boardState;

    void Start()
    {
        boardState = new int[lengthOfBoard, heightOfBoard];
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        StartTurnTimer();

        if (!player1Turn)
        {
            // If player 2's turn, simulate AI move
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
            UpdateTimerText();
        }
    }

    void StartTurnTimer()
    {
        timer = turnTime;
        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        timerText.text = Mathf.Ceil(timer).ToString();
    }

    public void HoverColumn(int column)
    {
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

    public void SelectColumn(int column)
    {
        if (!gamePaused && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero) && !playerWon)
        {
            TakeTurn(column);
        }
    }

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

            StartTurnTimer();
            
            // If it's AI's turn, simulate AI move
            if (!player1Turn && !playerWon)
            {
                int aiColumn = AISelectColumn();
                StartCoroutine(AIDelay(aiColumn));
            }
        }
    }

    IEnumerator AIDelay(int column)
    {
        yield return new WaitForSeconds(1f); // Delay the AI move to make it look more natural
        TakeTurn(column);
    }

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

    if (!player1Turn)
        {
        // If it's now player 2's turn, simulate AI move
        int aiColumn = AISelectColumn();
        StartCoroutine(AIDelay(aiColumn));
        }
    }

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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

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
