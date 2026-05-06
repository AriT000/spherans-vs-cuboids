using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    

    public void PlayGame(string scene_name)
    {
        SceneManager.LoadScene("demo_enemies_variation");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
