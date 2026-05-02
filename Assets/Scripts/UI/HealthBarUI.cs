using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Entities;
public class HealthBarUI : MonoBehaviour
{
    public float health, maxHealth, Width, Height;
    public Text healthText, MaxhealthText;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField] private AttributesManager attributesManager;

    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    void Start()
    {
        SetHealth(attributesManager.Health);
        attributesManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AttributesManager>();
    }
    
    //purpose: Set the health bar
    public void SetHealth(int health)
    {
        healthText.text = attributesManager.Health.ToString();
        MaxhealthText.text = attributesManager.Health.ToString();
    }

    //Purpose: Update the healthbar on callback on collision
    public void UpdateHealthBar(int currentHealth)
    {
        float newWidth = (currentHealth / maxHealth) * Width;
        healthBar.sizeDelta = new Vector2(newWidth,Height);
        healthText.text = currentHealth.ToString();
    }
}
