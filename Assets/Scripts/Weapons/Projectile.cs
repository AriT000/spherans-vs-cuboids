using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other projectiles
        if (other.CompareTag("EnemyProjectile") || other.CompareTag("PlayerProjectile"))
            return;

        // Ignore enemies (boss + other enemies)
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            return;

        Destroy(gameObject);
    }
}