using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour//Game Over Screen when player dies
{
    public void Setup()//makes the screen visible when player is at 0 health
    {
        gameObject.SetActive(true);
    }

    public void Restart()//when player clickes the restart button restarts the game
    {
        SceneManager.LoadScene("demo_enemies_variation");
    }

    public void Exit()//when player quits goes to main menu
    {
        SceneManager.LoadScene("MainMenu");
    }
}