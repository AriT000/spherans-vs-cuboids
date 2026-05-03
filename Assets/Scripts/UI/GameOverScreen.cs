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
        SceneManager.LoadScene("demo_enemies_variation");
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}