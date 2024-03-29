using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
   public int column;
   public GameManager gm;

   void OnMouseDown() 
   {
    Debug.Log("Column number is" + column);
    gm.SelectColumn(column);
    gm.TakeTurn(column);
   }
}
