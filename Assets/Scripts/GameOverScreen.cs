using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    
    public void Setup()
    {
        gameObject.SetActive(true);
    }
    
    public void RestartButton()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
