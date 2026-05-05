using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Entities;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("Boss Health")]
    public float health;
    public float maxHealth = 200f;

    [Header("UI")]
    public Text healthText;
    public Text MaxhealthText;

    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject bossHealthBarContainer;

    private float Width;
    private float Height;

    private AttributesManager attributesManager;
    public GameManager gameManager;

    void Start()
    {
        Width = healthBar.sizeDelta.x;
        Height = healthBar.sizeDelta.y;

        if (bossHealthBarContainer != null)
            bossHealthBarContainer.SetActive(false);
    }

    void Update()
    {
        if (attributesManager == null)
        {
            FindBoss();
            return;
        }

        UpdateHealthBar(attributesManager.Health);
    }

    private void FindBoss()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (boss == null)
        {
            if (bossHealthBarContainer != null)
                bossHealthBarContainer.SetActive(false);

            return;
        }

        attributesManager = boss.GetComponent<AttributesManager>();

        if (attributesManager != null)
        {
            health = attributesManager.Health;

            maxHealth = attributesManager.Health;

            healthText.text = health.ToString();
            MaxhealthText.text = maxHealth.ToString();

            if (bossHealthBarContainer != null)
                bossHealthBarContainer.SetActive(true);
        }
    }

    public void UpdateHealthBar(int currentHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        float newWidth = healthPercent * Width;

        newWidth = Mathf.Clamp(newWidth, 0, Width);

        healthBar.sizeDelta = new Vector2(newWidth, Height);
        healthText.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            if (bossHealthBarContainer != null)
                bossHealthBarContainer.SetActive(false);

            if (gameManager != null)
                gameManager.TriggerWin();
        }
    }
}