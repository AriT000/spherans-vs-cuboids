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

	private void Awake()
	{
		Instance = this;
	}

	// listen to wave and unit death events to track game state
	private void OnEnable()
	{
		AttributesManager.OnPlayerDeath += OnPlayerDeath;
		WaveManager.OnAllRoundsComplete += OnAllRoundsComplete;
	}

	private void OnDisable()
	{
		AttributesManager.OnPlayerDeath -= OnPlayerDeath;
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

	private void OnPlayerDeath()
	{
		TriggerGameOver();
	}

	private void OnAllRoundsComplete()
	{
		TriggerGameOver();
	}
}