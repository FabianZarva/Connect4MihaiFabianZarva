using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVsAIGameManager : MonoBehaviour
{
    // Player pieces
    public GameObject player1;
    public GameObject player2;

    // Spawn locations for pieces
    public GameObject[] spawnLoc;

    // Tracking the current player's turn
    bool player1Turn = true;

    // Tracking the state of the game board
    int[,] boardState;

    // Delay between turns (adjustable)
    public float turnDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the board state and start the game
        boardState = new int[7, 6]; // Assuming 7x6 board size
        StartGame();
    }

    // Start the game
    void StartGame()
    {
        player1Turn = true;
        // Start the game with the first AI player's turn
        StartCoroutine(AITurn());
    }

    // Coroutine for AI turn
    IEnumerator AITurn()
    {
        yield return new WaitForSeconds(turnDelay); // Delay the AI move
        int aiColumn = AISelectColumn(); // AI selects a column
        TakeTurn(aiColumn); // AI takes its turn
    }

    // Take a turn
    void TakeTurn(int column)
    {
        if (UpdateBoardState(column))
        {
            // Instantiate a new piece at the selected column's spawn location
            GameObject newPiece = Instantiate(player1Turn ? player1 : player2, spawnLoc[column].transform.position, Quaternion.identity);
            newPiece.transform.Rotate(Vector3.up, 90f); // Rotate the piece by 90 degrees on the y-axis

            // Start the next AI player's turn
            StartCoroutine(AITurn());
        }
        else if (IsBoardFull()) // Check if the board is full after the turn
        {
            ClearBoard(); // Clear the board if it's full
        }
    }

    // AI selects a column to drop a piece
    int AISelectColumn()
    {
        // Simple AI: randomly select a valid column
        List<int> validColumns = new List<int>();
        for (int i = 0; i < 7; i++) // Assuming 7 columns
        {
            if (boardState[i, 5] == 0)
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
        for (int row = 0; row < 6; row++) // Assuming 6 rows
        {
            if (boardState[column, row] == 0)
            {
                boardState[column, row] = player1Turn ? 1 : 2; // 1 for player 1, 2 for player 2
                player1Turn = !player1Turn; // Toggle player turn
                return true;
            }
        }
        Debug.LogWarning("Column " + column + " is full.");
        return false;
    }

    // Check if the board is full
    bool IsBoardFull()
    {
        for (int i = 0; i < 7; i++) // Assuming 7 columns
        {
            if (boardState[i, 5] == 0)
            {
                return false; // If any column has an empty space, the board is not full
            }
        }
        return true; // If no column has an empty space, the board is full
    }

    // Clear the board
    void ClearBoard()
    {
        boardState = new int[7, 6]; // Reset the board to all empty spaces
    }
}
