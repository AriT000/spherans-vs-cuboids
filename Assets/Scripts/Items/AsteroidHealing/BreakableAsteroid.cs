using System.Collections;
using UnityEngine;

public class BreakableAsteroid : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private int damagePerPlayerHit = 10;
    [SerializeField] private string playerProjectileTag = "PlayerProjectile";

    [Header("SFX")]
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip destroySfx;
    [SerializeField] private AudioClip spawnSfx;

    [Header("Break Pieces")]
    [SerializeField] private GameObject[] cornerPiecePrefabs;
    [SerializeField] private float pieceSpeed = 2.5f;
    [SerializeField] private float pieceLifetime = 3f;

    [Header("Drops")]
    [SerializeField] private GameObject blueCrystalPrefab;
    [SerializeField] private int minCrystalDrops = 3;
    [SerializeField] private int maxCrystalDrops = 4;
    [SerializeField] private float crystalSpawnRadius = 0.6f;

    [Header("Hit VFX")]
    [SerializeField] private EntityMaterials entityMaterials;
    [SerializeField] private float fallbackFlashLength = 0.08f;

    private Coroutine hitFlashRoutine;
    private int health;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private bool destroyed;

    //for SFX (and hit VFX)
    private AudioSource audioSource;
    

    private void Awake()
    {
        health = maxHealth;

        //added for SFX
        audioSource = GetComponent<AudioSource>();
        

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalMaterial = spriteRenderer.material;
        else
            Debug.LogWarning($"{name} has no SpriteRenderer for hit VFX.");
    }

    private void Start()
    {
        PlaySfx(spawnSfx);
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

        PlayHitVFX();

        if (health <= 0)
        {
            Break();
        }
        else
        {
            PlaySfx(hitSfx);
        }
    }

    private void PlayHitVFX()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name} cannot play hit VFX because SpriteRenderer is missing.");
            return;
        }

        if (entityMaterials == null)
        {
            Debug.LogWarning($"{name} cannot play hit VFX because EntityMaterials is missing.");
            return;
        }

        if (entityMaterials.hitEffectSprite2D == null)
        {
            Debug.LogWarning($"{name} cannot play hit VFX because hitEffectSprite2D is missing.");
            return;
        }

        if (hitFlashRoutine != null)
            StopCoroutine(hitFlashRoutine);

        hitFlashRoutine = StartCoroutine(PlayHitFlash());
    }

    private IEnumerator PlayHitFlash()
    {
        Material hitMaterial = entityMaterials.hitEffectSprite2D;

        spriteRenderer.material = hitMaterial;

        float flashLength = fallbackFlashLength;

        if (hitMaterial.HasProperty("_flashLength"))
            flashLength = hitMaterial.GetFloat("_flashLength");

        yield return new WaitForSeconds(flashLength);

        if (!destroyed && spriteRenderer != null)
        {
            if (entityMaterials.defaultSprite2D != null)
                spriteRenderer.material = entityMaterials.defaultSprite2D;
            else if (originalMaterial != null)
                spriteRenderer.material = originalMaterial;
        }

        hitFlashRoutine = null;
    }

    private void Break()
    {
        destroyed = true;

        PlaySfx(destroySfx);

        SpawnPieces();
        SpawnCrystals();

        //Hide sprite so it looks destroyed while auio plays
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;



        // Destroy the asteroid after the destroy SFX finishes
        float destroyDelay = destroySfx != null ? destroySfx.length : 0f;
        Destroy(gameObject, destroyDelay);
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

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
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
        if (blueCrystalPrefab == null)
        {
            Debug.LogWarning($"{name} cannot spawn crystals because Blue Crystal Prefab is missing.");
            return;
        }

        int safeMin = Mathf.Max(3, minCrystalDrops);
        int safeMax = Mathf.Max(safeMin, maxCrystalDrops);

        int dropCount = Random.Range(safeMin, safeMax + 1);

        Debug.Log($"{name} spawning {dropCount} crystals.");

        for (int i = 0; i < dropCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * crystalSpawnRadius;

            GameObject crystal = Instantiate(
                blueCrystalPrefab,
                transform.position + new Vector3(offset.x, offset.y, 0f),
                Quaternion.identity
            );

            Debug.Log($"Spawned crystal: {crystal.name} at {crystal.transform.position}");
        }
    }
}