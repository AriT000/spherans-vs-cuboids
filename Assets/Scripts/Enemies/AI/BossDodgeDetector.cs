using UnityEngine;

public class BossDodgeDetector : MonoBehaviour
{
    [SerializeField] private FinalBossEdgeController boss;

    private void Awake()
    {
        if (boss == null)
            boss = GetComponentInParent<FinalBossEdgeController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (boss != null)
            boss.TryDodgeFromBullet(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (boss != null)
            boss.TryDodgeFromBullet(other);
    }
}