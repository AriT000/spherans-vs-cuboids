using UnityEngine;

public class EnemyALaser : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Transform player;
    [SerializeField] private int bulletCount = 1;

    [SerializeField] private float fireRate = 1.2f;

    [SerializeField] private AudioSource shootAudio;

    private float timer;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 dir = player.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            timer = 0f;
            particleSystem.Emit(bulletCount);

            if (shootAudio != null)
            {
                shootAudio.Play();
            }
        }
    }
}