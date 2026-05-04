using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyEdgeFollower : MonoBehaviour
{
    private enum CameraEdge
    {
        Top,
        Bottom,
        Left,
        Right
    }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera targetCamera;

    [Header("Movement")]
    [SerializeField] private float attachSpeed = 10f;
    [SerializeField] private float strafeSpeed = 2.5f;
    [SerializeField] private float edgeInset = 1.0f;
    [SerializeField] private float snapTolerance = 0.15f;

    [Header("Strafing")]
    [SerializeField] private float strafeChangeIntervalMin = 1.0f;
    [SerializeField] private float strafeChangeIntervalMax = 2.0f;

    [Header("Facing")]
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private float spriteForwardOffset = -90f;

    [Header("SFX")]
    [SerializeField] private AudioClip strafeSfx;
    
    private Rigidbody2D rb;
    private Transform camTransform;

    private CameraEdge attachedEdge;

    private AudioSource audioSource;
    private bool isAttached;

    // Position along the chosen edge:
    // Top/Bottom use edgeAxis as X offset from camera center
    // Left/Right use edgeAxis as Y offset from camera center
    private float edgeAxis;
    private float currentStrafeDirection = 1f;
    private float nextStrafeChangeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camTransform = targetCamera.transform;

        DetermineInitialEdge();
        PickNewStrafeDirection();
    }

    void FixedUpdate()
    {
        if (targetCamera == null || camTransform == null)
            return;

        float camHalfHeight = targetCamera.orthographicSize;
        float camHalfWidth = camHalfHeight * targetCamera.aspect;
        Vector2 camPos = camTransform.position;
        Vector2 enemyPos = rb.position;

        float usableHalfWidth = Mathf.Max(0.01f, camHalfWidth - edgeInset);
        float usableHalfHeight = Mathf.Max(0.01f, camHalfHeight - edgeInset);

        Vector2 targetPos = GetAttachedWorldPosition(camPos, usableHalfWidth, usableHalfHeight);

        if (!isAttached)
        {
            Vector2 toTarget = targetPos - enemyPos;
            float dist = toTarget.magnitude;

            if (dist <= snapTolerance)
            {
                isAttached = true;
                rb.position = targetPos;
                rb.linearVelocity = Vector2.zero;

                StartStrafeSfx();
            }
            else
            {
                Vector2 desiredVelocity = toTarget.normalized * attachSpeed;
                rb.linearVelocity = desiredVelocity;
            }
        }
        else
        {
            if (Time.time >= nextStrafeChangeTime)
                PickNewStrafeDirection();

            UpdateEdgeAxis(usableHalfWidth, usableHalfHeight);

            targetPos = GetAttachedWorldPosition(camPos, usableHalfWidth, usableHalfHeight);

            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, attachSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            rb.linearVelocity = Vector2.zero;
        }

        if (facePlayer && playerTransform != null)
        {
            FaceTarget(playerTransform.position);
        }
        else if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            FaceTarget((Vector2)transform.position + rb.linearVelocity);
        }
    }

    private void DetermineInitialEdge()
    {
        if (camTransform == null)
            return;

        Vector2 camPos = camTransform.position;
        Vector2 enemyPos = transform.position;
        Vector2 fromCam = enemyPos - camPos;

        if (fromCam.sqrMagnitude < 0.0001f)
            fromCam = Vector2.up;

        float absX = Mathf.Abs(fromCam.x);
        float absY = Mathf.Abs(fromCam.y);

        if (absX > absY)
        {
            attachedEdge = fromCam.x >= 0f ? CameraEdge.Right : CameraEdge.Left;
            edgeAxis = fromCam.y;
        }
        else
        {
            attachedEdge = fromCam.y >= 0f ? CameraEdge.Top : CameraEdge.Bottom;
            edgeAxis = fromCam.x;
        }
    }

    private void UpdateEdgeAxis(float usableHalfWidth, float usableHalfHeight)
    {
        edgeAxis += currentStrafeDirection * strafeSpeed * Time.fixedDeltaTime;

        switch (attachedEdge)
        {
            case CameraEdge.Top:
            case CameraEdge.Bottom:
                if (edgeAxis > usableHalfWidth)
                {
                    edgeAxis = usableHalfWidth;
                    currentStrafeDirection = -1f;
                    PickNewStrafeTimeOnly();
                }
                else if (edgeAxis < -usableHalfWidth)
                {
                    edgeAxis = -usableHalfWidth;
                    currentStrafeDirection = 1f;
                    PickNewStrafeTimeOnly();
                }
                break;

            case CameraEdge.Left:
            case CameraEdge.Right:
                if (edgeAxis > usableHalfHeight)
                {
                    edgeAxis = usableHalfHeight;
                    currentStrafeDirection = -1f;
                    PickNewStrafeTimeOnly();
                }
                else if (edgeAxis < -usableHalfHeight)
                {
                    edgeAxis = -usableHalfHeight;
                    currentStrafeDirection = 1f;
                    PickNewStrafeTimeOnly();
                }
                break;
        }
    }

    private Vector2 GetAttachedWorldPosition(Vector2 camPos, float usableHalfWidth, float usableHalfHeight)
    {
        switch (attachedEdge)
        {
            case CameraEdge.Top:
                return new Vector2(
                    camPos.x + Mathf.Clamp(edgeAxis, -usableHalfWidth, usableHalfWidth),
                    camPos.y + usableHalfHeight
                );

            case CameraEdge.Bottom:
                return new Vector2(
                    camPos.x + Mathf.Clamp(edgeAxis, -usableHalfWidth, usableHalfWidth),
                    camPos.y - usableHalfHeight
                );

            case CameraEdge.Left:
                return new Vector2(
                    camPos.x - usableHalfWidth,
                    camPos.y + Mathf.Clamp(edgeAxis, -usableHalfHeight, usableHalfHeight)
                );

            case CameraEdge.Right:
                return new Vector2(
                    camPos.x + usableHalfWidth,
                    camPos.y + Mathf.Clamp(edgeAxis, -usableHalfHeight, usableHalfHeight)
                );

            default:
                return camPos;
        }
    }

    private void PickNewStrafeDirection()
    {
        currentStrafeDirection = Random.value < 0.5f ? -1f : 1f;
        nextStrafeChangeTime = Time.time + Random.Range(strafeChangeIntervalMin, strafeChangeIntervalMax);
    }

    private void PickNewStrafeTimeOnly()
    {
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

    private void StartStrafeSfx()
{
    if (audioSource == null || strafeSfx == null)
        return;

    if (!audioSource.isPlaying)
    {
        audioSource.clip = strafeSfx;
        audioSource.loop = true;
        audioSource.Play();
    }
}

private void StopStrafeSfx()
{
    if (audioSource != null && audioSource.isPlaying)
    {
        audioSource.Stop();
    }
}

private void OnDisable()
{
    StopStrafeSfx();
}

private void OnDestroy()
{
    StopStrafeSfx();
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