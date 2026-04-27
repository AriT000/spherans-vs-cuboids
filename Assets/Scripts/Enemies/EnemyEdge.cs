using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyEdgeFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera targetCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float edgeInset = 1.0f;
    [SerializeField] private float followLerpStrength = 8f;

    [Header("Strafing")]
    [SerializeField] private float strafeSpeed = 2f;
    [SerializeField] private float strafeChangeIntervalMin = 1.0f;
    [SerializeField] private float strafeChangeIntervalMax = 2.0f;

    [Header("Facing")]
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private float spriteForwardOffset = -90f;

    private Rigidbody2D rb;
    private Transform camTransform;

    private float currentStrafeDirection = 1f;
    private float nextStrafeChangeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camTransform = targetCamera.transform;

        PickNewStrafeDirection();
    }

    void FixedUpdate()
    {
        if (targetCamera == null || camTransform == null)
            return;

        Vector2 camPos = camTransform.position;
        Vector2 enemyPos = rb.position;

        Vector2 fromCamToEnemy = enemyPos - camPos;
        if (fromCamToEnemy.sqrMagnitude < 0.0001f)
            fromCamToEnemy = Vector2.up;

        Vector2 radialDir = fromCamToEnemy.normalized;
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x) * currentStrafeDirection;

        Vector2 targetEdgePoint = GetCameraEdgePoint(radialDir);

        Vector2 strafeOffset = tangentDir * strafeSpeed * 0.35f;
        Vector2 desiredTarget = targetEdgePoint + strafeOffset;

        Vector2 desiredVelocity = (desiredTarget - enemyPos) * followLerpStrength;

        if (desiredVelocity.magnitude > moveSpeed)
            desiredVelocity = desiredVelocity.normalized * moveSpeed;

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime
        );

        if (Time.time >= nextStrafeChangeTime)
            PickNewStrafeDirection();

        if (facePlayer && playerTransform != null)
        {
            FaceTarget(playerTransform.position);
        }
        else if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            FaceTarget((Vector2)transform.position + rb.linearVelocity);
        }

        ClampInsideCamera();
    }

    private void ClampInsideCamera()
    {
        float camHeight = targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        Vector3 camPos = camTransform.position;
        Vector2 pos = rb.position;

        float minX = camPos.x - camWidth + edgeInset;
        float maxX = camPos.x + camWidth - edgeInset;
        float minY = camPos.y - camHeight + edgeInset;
        float maxY = camPos.y + camHeight - edgeInset;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rb.position = pos;
    }

    private Vector2 GetCameraEdgePoint(Vector2 directionFromCenter)
    {
        float camHeight = targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        float usableHalfWidth = Mathf.Max(0.01f, camWidth - edgeInset);
        float usableHalfHeight = Mathf.Max(0.01f, camHeight - edgeInset);

        float scaleX = usableHalfWidth / Mathf.Abs(directionFromCenter.x == 0 ? 0.0001f : directionFromCenter.x);
        float scaleY = usableHalfHeight / Mathf.Abs(directionFromCenter.y == 0 ? 0.0001f : directionFromCenter.y);

        float scale = Mathf.Min(scaleX, scaleY);

        Vector2 camCenter = camTransform.position;
        return camCenter + directionFromCenter * scale;
    }

    private void PickNewStrafeDirection()
    {
        currentStrafeDirection = Random.value < 0.5f ? -1f : 1f;
        nextStrafeChangeTime = Time.time + Random.Range(strafeChangeIntervalMin, strafeChangeIntervalMax);
    }

    private void FaceTarget(Vector2 targetPos)
    {
        Vector2 dir = targetPos - (Vector2)transform.position;
        if (dir.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffset);
    }

    public void SetPlayerTransform(Transform target)
    {
        playerTransform = target;
    }

    public void SetCamera(Camera cam)
    {
        targetCamera = cam;
        camTransform = cam != null ? cam.transform : null;
    }
}