using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Entities;
public class BossHealthBarUI : MonoBehaviour
{
    public float health, maxHealth, Width, Height;
    public Text healthText, MaxhealthText;
    [SerializeField]
    private RectTransform healthBar;
    private AttributesManager attributesManager;
    public GameManager gameManager;


    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    void Start()
    {
        Width = healthBar.sizeDelta.x;
        Height = healthBar.sizeDelta.y;
        
        StartCoroutine(FindBoss());
    }
    
    IEnumerator FindBoss()
    {
        // Wait a few frames for boss to spawn
        yield return null;
        
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        while (boss == null)
        {
            yield return new WaitForSeconds(0.5f);
            boss = GameObject.FindGameObjectWithTag("Boss");
        }
        
        attributesManager = boss.GetComponent<AttributesManager>();
        if (attributesManager != null)
        {
            health = attributesManager.Health;
            maxHealth = 200;
            healthText.text = health.ToString();
            MaxhealthText.text = maxHealth.ToString();
        }
    }


public void UpdateHealthBar(int currentHealth)
{
    // Prevent division issues
    
    float newWidth = (currentHealth / maxHealth) * Width;
    
    // Clamp to avoid negative values
    newWidth = Mathf.Clamp(newWidth, 0, Width);
    
    healthBar.sizeDelta = new Vector2(newWidth, Height);
    healthText.text = currentHealth.ToString();
    if (currentHealth <= 0)
    {
        gameManager.TriggerWin();
    }
}
}
