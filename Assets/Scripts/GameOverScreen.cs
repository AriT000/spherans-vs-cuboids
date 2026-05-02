using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Demo_Scene");
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}