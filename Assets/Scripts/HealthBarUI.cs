using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Entities;
public class HealthBarUI : MonoBehaviour
{
    public float Health, MaxHealth, Width, Height;
    public Text healthText, MaxhealthText;
    [SerializeField]
    private RectTransform healthBar;
    private AttributesManager attributesManager;

    void Start()
    {
        SetMaxHealth();
        SetHealth(100);
    }
    public void SetMaxHealth()
    {
        MaxHealth = 100;
        MaxhealthText.text = MaxHealth.ToString();
    }

    public void SetHealth(int health)
    {
        Health = health;
        float newWidth = (Health / MaxHealth) * Width;
        healthBar.sizeDelta = new Vector2(newWidth,Height);
        healthText.text = Health.ToString();
    }

    public void UpdateHealthBar(int currentHealth)
    {
        float newWidth = (currentHealth / MaxHealth) * Width;
        healthBar.sizeDelta = new Vector2(newWidth,Height);
        healthText.text = currentHealth.ToString();
    }
}
