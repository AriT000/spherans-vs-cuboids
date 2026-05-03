using System.Collections;
using UnityEngine;
using Assets.Scripts.Entities;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyDiverEdgeAttack : MonoBehaviour
{
    private enum CameraEdge
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private enum DiverState
    {
        Attaching,
        Waiting,
        Diving,
        Returning
    }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera targetCamera;

    [Header("Edge Attach")]
    [SerializeField] private float attachSpeed = 10f;
    [SerializeField] private float strafeSpeed = 1f;
    [SerializeField] private float edgeInset = 1f;
    [SerializeField] private float snapTolerance = 0.15f;

    [Header("Dive")]
    [SerializeField] private float waitBeforeDive = 5f;
    [SerializeField] private float diveSpeed = 14f;
    [SerializeField] private float diveDistancePastPlayer = 4f;
    [SerializeField] private float reattachDelay = 0.25f;

    [Header("Strafing While Waiting")]
    [SerializeField] private float strafeChangeIntervalMin = 1f;
    [SerializeField] private float strafeChangeIntervalMax = 2.5f;

    [Header("Facing")]
    [SerializeField] private bool facePlayerWhileWaiting = true;
    [SerializeField] private float spriteForwardOffset = -90f;

    private Rigidbody2D rb;
    private Collider2D solidHurtbox;
    private Collider2D diveTrigger;
    private Collider2D hitbox;
    private bool ignoredPlayerCollision;
    private Transform camTransform;

    private CameraEdge attachedEdge;
    private DiverState state;

    private float edgeAxis;
    private float currentStrafeDirection = 1f;
    private float nextStrafeChangeTime;

    private Vector2 diveDirection;
    private Vector2 diveEndPoint;
    private bool damagedPlayerThisDive;
    private Coroutine diveRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            if (col.isTrigger)
                diveTrigger = col;
            else
                solidHurtbox = col;
        }

        hitbox = diveTrigger;
    }

    private void Start()
    {
        FindMissingReferences();

        DetermineInitialEdge();
        PickNewStrafeDirection();

        state = DiverState.Attaching;
    }

    private void FixedUpdate()
    {
        FindMissingReferences();

        if (targetCamera == null || camTransform == null)
            return;

        switch (state)
        {
            case DiverState.Attaching:
            case DiverState.Returning:
                MoveToCameraEdge();
                break;

            case DiverState.Waiting:
                StrafeOnCameraEdge();

                if (facePlayerWhileWaiting && playerTransform != null)
                    FaceTarget(playerTransform.position);

                break;

            case DiverState.Diving:
                MoveDive();
                break;
        }
    }

    private void MoveToCameraEdge()
    {
        Vector2 targetPos = GetAttachedWorldPosition();
        Vector2 toTarget = targetPos - rb.position;

        if (toTarget.magnitude <= snapTolerance)
        {
            rb.position = targetPos;
            rb.linearVelocity = Vector2.zero;
            state = DiverState.Waiting;

            if (diveRoutine != null)
                StopCoroutine(diveRoutine);

            diveRoutine = StartCoroutine(WaitThenDive());
            return;
        }

        rb.linearVelocity = toTarget.normalized * attachSpeed;

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
            FaceTarget(rb.position + rb.linearVelocity);
    }

    private void StrafeOnCameraEdge()
    {
        if (Time.time >= nextStrafeChangeTime)
            PickNewStrafeDirection();

        float camHalfHeight = targetCamera.orthographicSize;
        float camHalfWidth = camHalfHeight * targetCamera.aspect;

        float usableHalfWidth = Mathf.Max(0.01f, camHalfWidth - edgeInset);
        float usableHalfHeight = Mathf.Max(0.01f, camHalfHeight - edgeInset);

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

        Vector2 targetPos = GetAttachedWorldPosition();
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, attachSpeed * Time.fixedDeltaTime));
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator WaitThenDive()
    {
        yield return new WaitForSeconds(waitBeforeDive);

        FindMissingReferences();

        if (playerTransform == null)
        {
            Debug.LogWarning($"{name} cannot dive because playerTransform is missing. Make sure the Player object has tag 'Player'.");
            yield break;
        }

        StartDive();
    }

    private void StartDive()
    {
        state = DiverState.Diving;
        damagedPlayerThisDive = false;

        Vector2 startPos = rb.position;
        Vector2 lockedPlayerPosition = playerTransform.position;

        diveDirection = lockedPlayerPosition - startPos;

        if (diveDirection.sqrMagnitude < 0.001f)
            diveDirection = Vector2.down;
        else
            diveDirection.Normalize();

        float distanceToPlayer = Vector2.Distance(startPos, lockedPlayerPosition);
        diveEndPoint = startPos + diveDirection * (distanceToPlayer + diveDistancePastPlayer);

        rb.linearVelocity = diveDirection * diveSpeed;
        FaceTarget(startPos + diveDirection);
    }

    private void MoveDive()
    {
        rb.linearVelocity = diveDirection * diveSpeed;

        float remainingDistance = Vector2.Dot(diveEndPoint - rb.position, diveDirection);

        if (remainingDistance <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(ReattachAfterDelay());
        }
    }

    private IEnumerator ReattachAfterDelay()
    {
        state = DiverState.Returning;
        DetermineInitialEdge();

        yield return new WaitForSeconds(reattachDelay);
    }

    private Vector2 GetAttachedWorldPosition()
    {
        float camHalfHeight = targetCamera.orthographicSize;
        float camHalfWidth = camHalfHeight * targetCamera.aspect;

        float usableHalfWidth = Mathf.Max(0.01f, camHalfWidth - edgeInset);
        float usableHalfHeight = Mathf.Max(0.01f, camHalfHeight - edgeInset);

        Vector2 camPos = camTransform.position;

        switch (attachedEdge)
        {
            case CameraEdge.Top:
                return new Vector2(camPos.x + Mathf.Clamp(edgeAxis, -usableHalfWidth, usableHalfWidth), camPos.y + usableHalfHeight);

            case CameraEdge.Bottom:
                return new Vector2(camPos.x + Mathf.Clamp(edgeAxis, -usableHalfWidth, usableHalfWidth), camPos.y - usableHalfHeight);

            case CameraEdge.Left:
                return new Vector2(camPos.x - usableHalfWidth, camPos.y + Mathf.Clamp(edgeAxis, -usableHalfHeight, usableHalfHeight));

            case CameraEdge.Right:
                return new Vector2(camPos.x + usableHalfWidth, camPos.y + Mathf.Clamp(edgeAxis, -usableHalfHeight, usableHalfHeight));

            default:
                return camPos;
        }
    }

    private void DetermineInitialEdge()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        camTransform = targetCamera.transform;

        Vector2 camPos = camTransform.position;
        Vector2 enemyPos = transform.position;
        Vector2 fromCam = enemyPos - camPos;

        if (fromCam.sqrMagnitude < 0.0001f)
            fromCam = Vector2.up;

        if (Mathf.Abs(fromCam.x) > Mathf.Abs(fromCam.y))
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state != DiverState.Diving)
            return;

        if (damagedPlayerThisDive)
            return;

        if (!other.CompareTag("Player"))
            return;

        AttributesManager diverAttributes = GetComponent<AttributesManager>();
        AttributesManager playerAttributes = other.GetComponent<AttributesManager>();

        if (playerAttributes == null)
            playerAttributes = other.GetComponentInParent<AttributesManager>();

        if (diverAttributes == null || playerAttributes == null)
            return;

        damagedPlayerThisDive = true;

        int damage = GetPrivateDamagePower(diverAttributes);
        CallPrivateTakeDamage(playerAttributes, damage);
    }

    private int GetPrivateDamagePower(AttributesManager attributes)
    {
        System.Reflection.FieldInfo damageField =
            typeof(AttributesManager).GetField(
                "damagePower",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

        if (damageField == null)
            return 1;

        return (int)damageField.GetValue(attributes);
    }

    private void CallPrivateTakeDamage(AttributesManager attributes, int damage)
    {
        System.Reflection.MethodInfo takeDamageMethod =
            typeof(AttributesManager).GetMethod(
                "takeDamage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

        if (takeDamageMethod == null)
            return;

        takeDamageMethod.Invoke(attributes, new object[] { damage });
    }

    private void FaceTarget(Vector2 targetPos)
    {
        Vector2 dir = targetPos - (Vector2)transform.position;

        if (dir.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffset);
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

    public void SetPlayerTransform(Transform target)
    {
        playerTransform = target;
    }

    public void SetCamera(Camera cam)
    {
        targetCamera = cam;
        camTransform = cam != null ? cam.transform : null;
    }

    private void FindMissingReferences()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camTransform = targetCamera.transform;

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                playerTransform = playerObj.transform;
        }
        IgnoreSolidCollisionWithPlayer();
    }

    private void IgnoreSolidCollisionWithPlayer()
    {
        if (ignoredPlayerCollision)
            return;

        if (solidHurtbox == null || playerTransform == null)
            return;

        Collider2D[] playerColliders = playerTransform.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D playerCol in playerColliders)
        {
            Physics2D.IgnoreCollision(solidHurtbox, playerCol, true);
        }

        ignoredPlayerCollision = true;
    }
}