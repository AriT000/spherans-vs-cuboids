using UnityEngine;

public class HostageCompanion : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Vector2 followOffset = new Vector2(1.4f, 0.5f);
    [SerializeField] private float followSmoothTime = 0.12f;

    [Header("Combat")]
    [SerializeField] private GameObject weaponRoot;
    [SerializeField] private MonoBehaviour[] weaponBehaviours;
    [SerializeField] private LayerMask enemyLayerMask = ~0;

    [Header("Detection")]
    [SerializeField] private bool drawDetectionGizmo;

    private Transform player;
    private Camera mainCamera;

    private bool isFreed;
    private Vector3 followVelocity;

    private Transform currentTarget;

    public void Initialize(
        Transform playerTransform,
        Camera cam,
        Vector2 newFollowOffset,
        float newFollowSmoothTime,
        LayerMask newEnemyLayerMask
    )
    {
        player = playerTransform;
        mainCamera = cam;

        followOffset = newFollowOffset;
        followSmoothTime = Mathf.Max(0.01f, newFollowSmoothTime);
        enemyLayerMask = newEnemyLayerMask;

        LockHostage();
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        LockHostage();
    }

    private void Update()
    {
        if (!isFreed || player == null)
            return;

        UpdateFollow();
        UpdateTargeting();
        UpdateWeaponState();
    }

    public void LockHostage()
    {
        isFreed = false;
        SetWeaponActive(false);
    }

    public void FreeHostage()
    {
        isFreed = true;
        followVelocity = Vector3.zero;
    }

    private void UpdateFollow()
    {
        Vector3 targetPosition = player.position + (Vector3)GetResolvedOffset();
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            followSmoothTime
        );
    }

    private Vector2 GetResolvedOffset()
    {
        float xDirection = player.localScale.x >= 0f ? 1f : -1f;
        return new Vector2(Mathf.Abs(followOffset.x) * xDirection, followOffset.y);
    }

    //purpose: find closest enemy inside camera view
    private void UpdateTargeting()
    {
        if (mainCamera == null)
            return;

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            mainCamera.transform.position,
            new Vector2(camWidth * 2f, camHeight * 2f),
            0f,
            enemyLayerMask
        );

        float closestDist = float.MaxValue;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            if (hit == null)
                continue;

            Transform hitRoot = hit.transform.root;

            if (player != null && hitRoot == player.root)
                continue;

            if (hitRoot == transform.root)
                continue;

            if (hit.GetComponentInParent<SpawnedEnemyTracker>() == null)
                continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }

    //purpose: enemy detection | hostage can sonly shoot when freed
    private void UpdateWeaponState()
    {
        bool enemyDetected = currentTarget != null;

        SetWeaponActive(enemyDetected);

        if (weaponBehaviours == null) return;
        foreach (MonoBehaviour behaviour in weaponBehaviours)
        {
            if (behaviour == null) continue;

            if (behaviour is HostageLaserWeapon hostageWeapon)
            {
                hostageWeapon.SetTarget(currentTarget);
                hostageWeapon.SetCanFire(enemyDetected);
            }
        }
    }

    private void SetWeaponActive(bool active)
    {
        if (weaponRoot != null)
            weaponRoot.SetActive(active);

        if (weaponBehaviours == null)
            return;

        foreach (MonoBehaviour behaviour in weaponBehaviours)
        {
            if (behaviour != null)
                behaviour.enabled = active;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDetectionGizmo || Camera.main == null) return;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            Camera.main.transform.position,
            new Vector3(camWidth * 2f, camHeight * 2f, 0f)
        );
    }
}