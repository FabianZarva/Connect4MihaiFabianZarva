using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName; // Name of the scene to load

    // Load scene on button press
    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(sceneName); // Load the desired scene
    }
}
