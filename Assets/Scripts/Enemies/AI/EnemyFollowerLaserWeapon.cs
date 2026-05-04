using UnityEngine;

public class EnemyFollowerLaserWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Firing")]
    [SerializeField] private float fireRate = 2.0f;
    [SerializeField] private float spawnDistanceFromEnemy = 0.5f;

    private float timer;

    private void Start()
    {
        FindPlayerIfMissing();
    }

    private void Update()
    {
        FindPlayerIfMissing();

        if (player == null || projectilePrefab == null)
            return;

        Vector2 dir = player.position - transform.position;

        if (dir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            timer = 0f;
            FireFollowerShot();
        }
    }

    private void FireFollowerShot()
    {
        Vector2 dir = player.position - transform.position;

        if (dir.sqrMagnitude < 0.001f)
            dir = transform.right;
        else
            dir.Normalize();

        Vector3 spawnPos = transform.position + (Vector3)(dir * spawnDistanceFromEnemy);

        GameObject shot = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        FollowerLaserProjectile projectile = shot.GetComponent<FollowerLaserProjectile>();

        if (projectile != null)
            projectile.Initialize(player);
    }

    private void FindPlayerIfMissing()
    {
        if (player != null)
            return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    public void SetPlayerTransform(Transform target)
    {
        player = target;
    }
}