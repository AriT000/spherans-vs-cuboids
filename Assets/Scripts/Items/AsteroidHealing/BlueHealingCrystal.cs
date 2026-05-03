using UnityEngine;
using Assets.Scripts.Entities;

public class BlueHealingCrystal : MonoBehaviour
{
    [SerializeField] private int healAmount = 40;
    [SerializeField] private float driftSpeed = 0.75f;
    [SerializeField] private float rotateSpeed = 45f;
    [SerializeField] private float lifetime = 12f;

    private Vector2 driftDirection;

    private void Awake()
    {
        driftDirection = Random.insideUnitCircle.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(driftDirection * driftSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        AttributesManager playerAttributes = other.GetComponentInParent<AttributesManager>();

        if (playerAttributes != null)
        {
            playerAttributes.Health += healAmount;

            HealthBarUI healthBarUI = GameObject.FindWithTag("HudManager").GetComponent<HealthBarUI>();
            if (healthBarUI != null)
                healthBarUI.UpdateHealthBar(playerAttributes.Health);
        }

        Destroy(gameObject);
    }
}