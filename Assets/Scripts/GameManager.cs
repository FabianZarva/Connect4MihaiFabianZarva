using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject[] spawnLoc;
    
    bool player1Turn = true;
    int[,] boardState; // 0 = empty , 1 = player 1 , 2 = player 2

    // Start is called before the first frame update
    void Start()
    {
      boardState = new int [lengthOfBoard, heightOfBoard];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectColumn(int column)
    {
        Debug.Log("GameManager column" + column);
    }

    public void TakeTurn(int column)
    {
        if(UpdateBoardState(column));
        {

        if (player1Turn)
        {
            // Instantiate the player game object
            GameObject newPlayer = Instantiate(player1, spawnLoc[column].transform.position, Quaternion.identity);
            player1Turn = false;
            // Rotate the player game object around the y-axis by 90 degrees
            newPlayer.transform.Rotate(Vector3.up, 90f);
        }
        else
        {
            // Instantiate the player game object
            GameObject newPlayer = Instantiate(player2, spawnLoc[column].transform.position, Quaternion.identity);

            // Rotate the player game object around the y-axis by 90 degrees
            newPlayer.transform.Rotate(Vector3.up, 90f);

            player1Turn = true;
        }

        }
    }

    bool UpdateBoardState(int column) 
{
    for (int row = 0; row < heightOfBoard; row++)
    {
        if(boardState[column, row] == 0)
        {
            if(player1Turn)
            {
                boardState[column, row] = 1;
            }
            else
            {
                boardState[column, row] = 2;
            }

            Debug.Log("Piece is spawned at (" + column + ", " + row + ")");
           return true;
        }
    }
    Debug.LogWarning("isFull");
    return false;
}
}
