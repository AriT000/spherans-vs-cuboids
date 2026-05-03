using System.Collections;
using UnityEngine;

public class BreakableAsteroid : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damagePerPlayerHit = 10;
    [SerializeField] private string playerProjectileTag = "PlayerProjectile";

    [Header("Break Pieces")]
    [SerializeField] private GameObject[] cornerPiecePrefabs;
    [SerializeField] private float pieceSpeed = 2.5f;
    [SerializeField] private float pieceLifetime = 3f;

    [Header("Drops")]
    [SerializeField] private GameObject blueCrystalPrefab;
    [SerializeField] private int crystalDropCount = 3;
    [SerializeField] private float crystalSpawnRadius = 0.6f;

    [Header("Hit VFX")]
    [SerializeField] private EntityMaterials entityMaterials;

    private int health;
    private SpriteRenderer spriteRenderer;
    private bool destroyed;

    private void Awake()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag(playerProjectileTag))
            return;

        TakeDamage(damagePerPlayerHit);
    }

    public void TakeDamage(int damage)
    {
        if (destroyed) return;

        health -= damage;

        if (entityMaterials != null && spriteRenderer != null)
            StartCoroutine(PlayHitFlash());

        if (health <= 0)
            Break();
    }

    private IEnumerator PlayHitFlash()
    {
        Material defaultMaterial = entityMaterials.defaultSprite2D;
        Material hitMaterial = entityMaterials.hitEffectSprite2D;

        spriteRenderer.material = hitMaterial;

        float flashLength = hitMaterial.GetFloat("_flashLength");
        yield return new WaitForSeconds(flashLength);

        spriteRenderer.material = defaultMaterial;
    }

    private void Break()
    {
        destroyed = true;

        SpawnPieces();
        SpawnCrystals();

        Destroy(gameObject);
    }

    private void SpawnPieces()
    {
        if (cornerPiecePrefabs == null) return;

        for (int i = 0; i < cornerPiecePrefabs.Length; i++)
        {
            if (cornerPiecePrefabs[i] == null) continue;

            GameObject piece = Instantiate(cornerPiecePrefabs[i], transform.position, Quaternion.identity);

            Vector2 dir = GetCornerDirection(i);
            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = dir * pieceSpeed;

            Destroy(piece, pieceLifetime);
        }
    }

    private Vector2 GetCornerDirection(int index)
    {
        switch (index)
        {
            case 0: return new Vector2(-1f, 1f).normalized;
            case 1: return new Vector2(1f, 1f).normalized;
            case 2: return new Vector2(-1f, -1f).normalized;
            case 3: return new Vector2(1f, -1f).normalized;
            default: return Random.insideUnitCircle.normalized;
        }
    }

    private void SpawnCrystals()
    {
        if (blueCrystalPrefab == null) return;

        for (int i = 0; i < crystalDropCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * crystalSpawnRadius;
            Instantiate(
                blueCrystalPrefab,
                transform.position + new Vector3(offset.x, offset.y, 0f),
                Quaternion.identity
            );
        }
    }
}