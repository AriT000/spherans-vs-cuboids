using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    

    public void PlayGame(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
