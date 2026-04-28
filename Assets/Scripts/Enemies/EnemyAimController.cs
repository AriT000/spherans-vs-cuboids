using UnityEngine;

public class EnemyAimController : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject player;
    [SerializeField] private float aimRadius = 1f;
    [SerializeField] private float angleOffset = -90f;

    private Vector2 enemyPos;
    private Vector2 playerPos;
    private Vector2 currentAimPos;

    void Update()
    {
        if (enemy == null || player == null)
            return;

        enemyPos = enemy.transform.position;
        playerPos = player.transform.position;

        currentAimPos = GetAimPos(enemyPos, playerPos);
        transform.position = currentAimPos;

        getPlayerPos();
    }

    private Vector2 GetAimPos(Vector2 enemyCenter, Vector2 targetPos)
    {
        Vector2 direction = (targetPos - enemyCenter).normalized;
        Vector2 aimPos = enemyCenter + (direction * aimRadius);
        return aimPos;
    }

    private void getPlayerPos()
    {
        Vector2 direction = playerPos - (Vector2)transform.position;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset);
    }
}