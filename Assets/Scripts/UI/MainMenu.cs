using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    

    public void PlayGame(string scene_name)
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
        UnityEngine.Debug.Log("Ran playgame");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
