using UnityEngine;

public class HostageLaserWeapon : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Transform targetEnemy;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private bool canFire = false;

    private float timer;

    private void Awake()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!canFire || targetEnemy == null)
            return;

        Vector2 dir = targetEnemy.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            timer = 0f;

            if (particleSystem != null)
                particleSystem.Emit(1);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetEnemy = newTarget;
    }

    public void SetCanFire(bool value)
    {
        canFire = value;
    }
}