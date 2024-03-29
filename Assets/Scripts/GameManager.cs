using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player1;
    public GameObject player2;
    public GameObject[] spawnLoc;

    // Start is called before the first frame update
    void Start()
    {
        
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
    // Instantiate the player game object
    GameObject newPlayer = Instantiate(player1, spawnLoc[column].transform.position, Quaternion.identity);
    
    // Rotate the player game object around the y-axis by 90 degrees
    newPlayer.transform.Rotate(Vector3.up, 90f);
}
}
