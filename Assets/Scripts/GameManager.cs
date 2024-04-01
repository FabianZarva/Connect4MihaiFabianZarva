using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject player1;
    public GameObject player2;

    public GameObject player1Ghost;
    public GameObject player2Ghost;

    public GameObject winScreen;
    public Text winText;

    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject[] spawnLoc;

    GameObject fallingPiece;

    bool player1Turn = true;

    bool playerWon = false;
    int[,] boardState; // 0 = empty , 1 = player 1 , 2 = player 2

    // Start is called before the first frame update
    void Start()
    {
        boardState = new int[lengthOfBoard, heightOfBoard];

        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
    }

    public void HoverColumn(int column)
    {
        if (boardState[column, heightOfBoard - 1] == 0 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero) && !playerWon)
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
        if (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero && !playerWon)
        {
           // Debug.Log("GameManager column" + column);
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
                // Instantiate the player game object
                fallingPiece = Instantiate(player1, spawnLoc[column].transform.position, Quaternion.identity);
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
                player1Turn = false;
                // Rotate the player game object around the y-axis by 90 degrees
                fallingPiece.transform.Rotate(Vector3.up, 90f);

                if (Didwin(1))
                {   
                    playerWon = true;
                    winScreen.SetActive(true);
                    Debug.LogWarning("Player 1 Won!");
                    winText.text = "Player 1 Won!";
                    winText.color = Color.yellow;
                }
            }
            else
            {
                // Instantiate the player game object
                fallingPiece = Instantiate(player2, spawnLoc[column].transform.position, Quaternion.identity);
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
                 player1Turn = true;
                // Rotate the player game object around the y-axis by 90 degrees
                fallingPiece.transform.Rotate(Vector3.up, 90f);

               if (Didwin(2))
                {
                    playerWon = true;
                    winScreen.SetActive(true);
                    Debug.LogWarning("Player 2 Won!");
                    winText.text = "Player 2 Won!";
                    winText.color = Color.red;
                }
            }
            if (DidDraw())
            {
                playerWon = true; 
                winScreen.SetActive(true);
                Debug.LogWarning("Draw!");
                winText.text = "It's a draw this time!";

            }
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

              //  Debug.Log("Piece is spawned at (" + column + ", " + row + ")");
                return true;
            }
        }
        Debug.LogWarning("Column" + column + "isFull");
        return false;
    }

    bool Didwin(int playerNum)
    {
    // Horizontal
    for (int x = 0; x < lengthOfBoard - 3; x++) // Adjusted loop limits to prevent out of bounds access
    {
        for (int y = 0; y < heightOfBoard; y++)
        {
            if (boardState[x, y] == playerNum && boardState[x + 1, y] == playerNum && boardState[x + 2, y] == playerNum && boardState[x + 3, y] == playerNum)
            {
               return true;
            }
        }
    }

    // Vertical
    for (int x = 0; x < lengthOfBoard; x++)
    {
        for (int y = 0; y < heightOfBoard - 3; y++) // Adjusted loop limits to prevent out of bounds access
        {
            if (boardState[x, y] == playerNum && boardState[x, y + 1] == playerNum && boardState[x, y + 2] == playerNum && boardState[x, y + 3] == playerNum)
            {
                
                return true;
            }
        }
    }

    // y = x line
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

    // y = -x line
    for (int x = 0; x < lengthOfBoard - 3; x++)
    {
        for (int y = 3; y < heightOfBoard; y++) // Adjusted starting y index to prevent out of bounds access
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
        for( int x = 0; x < lengthOfBoard; x++)
        {
            if(boardState[x, heightOfBoard - 1] == 0)
            {
                return false;
            }
        }
        return true;
    }

   public void RestartGame()
   {
    SceneManager.LoadScene(0);
   }

   public void QuitGame()
        {
            Application.Quit();
        }
}
