using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    public int column; // The column index associated with this input field
    public GameManager gm; // Reference to the GameManager script

    // Called when the mouse button is pressed down over the input field
    void OnMouseDown() 
    {
        gm.SelectColumn(column); // Call the SelectColumn method in the GameManager, passing the column index
    }

    // Called when the mouse cursor hovers over the input field
    void OnMouseOver()
    {
        gm.HoverColumn(column); // Call the HoverColumn method in the GameManager, passing the column index
    }
}
