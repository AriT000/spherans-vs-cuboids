// Game manager script for tracking game state (win, lose)
using UnityEngine;
using Assets.Scripts.Entities;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] private GameOverScreen gameOverScreen;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject healthBar;
	[SerializeField] private GameObject weaponHeatBar;
	[SerializeField] private GameObject playerPortrait;
	[SerializeField] private GameObject winScene;
	[SerializeField] private GameObject waveManager;

	private void Awake()
	{
		Instance = this;
	}

	// listen to wave and unit death events to track game state
	private void OnEnable()
	{
		AttributesManager.OnPlayerDeath += OnPlayerDeath;
		AttributesManager.OnBossDeath += OnBossDeath;
		WaveManager.OnAllRoundsComplete += OnAllRoundsComplete;
	}

	private void OnDisable()
	{
		AttributesManager.OnPlayerDeath -= OnPlayerDeath;
		AttributesManager.OnBossDeath -= OnBossDeath;
		WaveManager.OnAllRoundsComplete -= OnAllRoundsComplete;
	}

	private void TriggerGameOver()
	{
		player.SetActive(false);
		healthBar.SetActive(false);
		weaponHeatBar.SetActive(false);
		playerPortrait.SetActive(false);
		gameOverScreen.Setup();
	}

	public void TriggerWin()
	{
		player.SetActive(false);
		healthBar.SetActive(false);
		weaponHeatBar.SetActive(false);
		playerPortrait.SetActive(false);
		winScene.SetActive(true);
		waveManager.SetActive(false);
    }

	private void OnPlayerDeath()
	{
		TriggerGameOver();
	}

	private void OnBossDeath()
	{
		TriggerWin();
	}

	private void OnAllRoundsComplete()
	{
		TriggerWin();
	}
}