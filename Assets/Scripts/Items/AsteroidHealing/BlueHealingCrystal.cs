using UnityEngine;
using Assets.Scripts.Entities;

public class BlueHealingCrystal : MonoBehaviour
{
    [SerializeField] private int healAmount = 40;
    [SerializeField] private float driftSpeed = 0.75f;
    [SerializeField] private float rotateSpeed = 45f;
    [SerializeField] private float lifetime = 12f;

    [Header("SFX")]
    [SerializeField] private AudioClip pickupSfx;

    private Vector2 driftDirection;

    //for SFX and pickup logic
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Collider2D crystalCollider;

    private bool pickedUp;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        crystalCollider = GetComponent<Collider2D>();

        driftDirection = Random.insideUnitCircle.normalized;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (pickedUp)
            return;

        transform.position += (Vector3)(driftDirection * driftSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        pickedUp = true;

        AttributesManager playerAttributes = other.GetComponentInParent<AttributesManager>();

        if (playerAttributes != null)
        {
            playerAttributes.Health += healAmount;

            HealthBarUI healthBarUI = GameObject.FindWithTag("HudManager").GetComponent<HealthBarUI>();
            if (healthBarUI != null)
                healthBarUI.UpdateHealthBar(playerAttributes.Health);
        }

        if (audioSource != null && pickupSfx != null)
        {
            audioSource.PlayOneShot(pickupSfx);
        }

        // Disable visuals so the player can see the pickup SFX, but prevent multiple pickups
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Disable collider to prevent repeat pickups
        if (crystalCollider != null)
            crystalCollider.enabled = false;

        Debug.Log($"Crystal picked up by: {other.name}, tag: {other.tag}");

        // Destroy the crystal after the pickup SFX finishes
        float destroyDelay = pickupSfx != null ? pickupSfx.length : 0f;
        Destroy(gameObject, destroyDelay);
    }
}