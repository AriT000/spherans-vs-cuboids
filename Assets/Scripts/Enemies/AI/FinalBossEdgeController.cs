using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FinalBossEdgeController : MonoBehaviour
{
    private enum CameraEdge { Top, Bottom, Left, Right }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform firePoint;

    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject fastProjectilePrefab;
    [SerializeField] private GameObject slowProjectilePrefab;
    [SerializeField] private GameObject shotgunProjectilePrefab;

    [Header("Projectile Speeds")]
    [SerializeField] private float fastProjectileSpeed = 14f;
    [SerializeField] private float slowProjectileSpeed = 6f;
    [SerializeField] private float shotgunProjectileSpeed = 9f;

    [Header("Edge Movement")]
    [SerializeField] private float attachSpeed = 16f;
    [SerializeField] private float strafeSpeed = 7f;
    [SerializeField] private float edgeInset = 1.1f;
    [SerializeField] private float snapTolerance = 0.15f;
    [SerializeField] private float edgeSwitchMinTime = 6f;
    [SerializeField] private float edgeSwitchMaxTime = 15f;

    [Header("Strafe Randomness")]
    [SerializeField] private float strafeChangeIntervalMin = 0.4f;
    [SerializeField] private float strafeChangeIntervalMax = 1.2f;

    [Header("Dodge")]
    [SerializeField] private string playerBulletTag = "PlayerProjectile";
    [SerializeField] private float dodgeChance = 0.2f;
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeSpeed = 28f;
    [SerializeField] private float dodgeCooldown = 0.8f;

    [Header("Attacks")]
    [SerializeField] private float timeBetweenMovesets = 0.7f;
    [SerializeField] private float timeBetweenShots = 0.12f;
    [SerializeField] private float shotgunSpreadAngle = 18f;
    [SerializeField] private int shotgunPellets = 5;

    [Header("Facing")]
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private float spriteForwardOffset = -90f;

    [Header("Body Collision VFX")]
    [SerializeField] private GameObject bodyHitVfx;

    private Rigidbody2D rb;
    private Transform camTransform;

    private CameraEdge attachedEdge = CameraEdge.Right;
    private float edgeAxis;
    private float currentStrafeDirection = 1f;
    private float nextStrafeChangeTime;
    private float nextEdgeSwitchTime;
    private float nextDodgeTime;

    private bool isAttached;
    private bool isDodging;
    private bool isAttacking;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camTransform = targetCamera.transform;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                player = GameObject.Find("Player");

            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogError("Final Boss could not find Player. Check Player tag/name.");
        }

        attachedEdge = CameraEdge.Right;
        edgeAxis = 0f;

        PickNewStrafeDirection();
        PickNextEdgeSwitchTime();

        StartCoroutine(BossAttackLoop());
    }

    private void FixedUpdate()
    {
        if (targetCamera == null || camTransform == null || isDodging)
            return;

        float camHalfHeight = targetCamera.orthographicSize;
        float camHalfWidth = camHalfHeight * targetCamera.aspect;

        float usableHalfWidth = Mathf.Max(0.01f, camHalfWidth - edgeInset);
        float usableHalfHeight = Mathf.Max(0.01f, camHalfHeight - edgeInset);

        Vector2 camPos = camTransform.position;

        if (Time.time >= nextEdgeSwitchTime)
            SwitchToRandomNewEdge();

        Vector2 targetPos = GetAttachedWorldPosition(camPos, usableHalfWidth, usableHalfHeight);

        if (!isAttached)
        {
            Vector2 toTarget = targetPos - rb.position;

            if (toTarget.magnitude <= snapTolerance)
            {
                isAttached = true;
                rb.MovePosition(targetPos);
            }
            else
            {
                rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, attachSpeed * Time.fixedDeltaTime));
            }
        }
        else
        {
            if (Time.time >= nextStrafeChangeTime)
                PickNewStrafeDirection();

            UpdateEdgeAxis(usableHalfWidth, usableHalfHeight);

            targetPos = GetAttachedWorldPosition(camPos, usableHalfWidth, usableHalfHeight);
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, attachSpeed * Time.fixedDeltaTime));
            rb.linearVelocity = Vector2.zero;
        }

        Face();
    }

    private IEnumerator BossAttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Debug.Log("Boss attack loop running. Player found: " + (playerTransform != null));
            if (!isAttacking && playerTransform != null)
            {
                isAttacking = true;

                if (attachedEdge == CameraEdge.Left || attachedEdge == CameraEdge.Right)
                    yield return StartCoroutine(UseSideMoveset());
                else
                    yield return StartCoroutine(UseVerticalMoveset());

                isAttacking = false;
            }

            yield return new WaitForSeconds(timeBetweenMovesets);
        }
    }

    private IEnumerator UseSideMoveset()
    {
        int choice = Random.Range(0, 3);

        switch (choice)
        {
            case 0:
                yield return StartCoroutine(TwoFastFiveSlowTwoShotgun());
                break;

            case 1:
                yield return StartCoroutine(SideCrossfireRush());
                break;

            case 2:
                yield return StartCoroutine(SideShotgunPressure());
                break;
        }
    }

    private IEnumerator UseVerticalMoveset()
    {
        int choice = Random.Range(0, 3);

        switch (choice)
        {
            case 0:
                yield return StartCoroutine(VerticalRainBurst());
                break;

            case 1:
                yield return StartCoroutine(VerticalSlowWall());
                break;

            case 2:
                yield return StartCoroutine(VerticalExecutionCombo());
                break;
        }
    }

    // Side-only moveset.
    private IEnumerator TwoFastFiveSlowTwoShotgun()
    {
        for (int i = 0; i < 2; i++)
        {
            FireAtPlayer(fastProjectilePrefab, fastProjectileSpeed);
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 5; i++)
        {
            FireAtPlayer(slowProjectilePrefab, slowProjectileSpeed);
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 2; i++)
        {
            FireShotgun();
            yield return new WaitForSeconds(0.35f);
        }
    }

    // Side-only moveset.
    private IEnumerator SideCrossfireRush()
    {
        FireShotgun();
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 4; i++)
        {
            FireAtPlayer(fastProjectilePrefab, fastProjectileSpeed);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.25f);
        FireShotgun();
    }

    // Side-only moveset.
    private IEnumerator SideShotgunPressure()
    {
        for (int i = 0; i < 3; i++)
        {
            FireShotgun();
            yield return new WaitForSeconds(0.45f);
        }

        for (int i = 0; i < 3; i++)
        {
            FireAtPlayer(slowProjectilePrefab, slowProjectileSpeed);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Top/bottom-only moveset.
    private IEnumerator VerticalRainBurst()
    {
        for (int i = 0; i < 8; i++)
        {
            FireAtPlayer(slowProjectilePrefab, slowProjectileSpeed);
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(0.25f);
        FireShotgun();
    }

    // Top/bottom-only moveset.
    private IEnumerator VerticalSlowWall()
    {
        for (int i = 0; i < 6; i++)
        {
            FireSpreadProjectile(slowProjectilePrefab, slowProjectileSpeed, -25f + i * 10f);
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 2; i++)
        {
            FireAtPlayer(fastProjectilePrefab, fastProjectileSpeed);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Top/bottom-only moveset.
    private IEnumerator VerticalExecutionCombo()
    {
        FireShotgun();
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 3; i++)
        {
            FireAtPlayer(fastProjectilePrefab, fastProjectileSpeed);
            yield return new WaitForSeconds(0.12f);
        }

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 4; i++)
        {
            FireAtPlayer(slowProjectilePrefab, slowProjectileSpeed);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FireAtPlayer(GameObject prefab, float speed)
    {
        if (prefab == null || playerTransform == null)
            return;

        Vector2 direction = ((Vector2)playerTransform.position - GetFirePosition()).normalized;
        SpawnProjectile(prefab, direction, speed);
    }

    private void FireShotgun()
    {
        if (shotgunProjectilePrefab == null || playerTransform == null)
            return;

        Vector2 baseDirection = ((Vector2)playerTransform.position - GetFirePosition()).normalized;

        for (int i = 0; i < shotgunPellets; i++)
        {
            float t = shotgunPellets <= 1 ? 0.5f : i / (float)(shotgunPellets - 1);
            float angle = Mathf.Lerp(-shotgunSpreadAngle, shotgunSpreadAngle, t);
            Vector2 direction = RotateVector(baseDirection, angle);
            SpawnProjectile(shotgunProjectilePrefab, direction, shotgunProjectileSpeed);
        }
    }

    private void FireSpreadProjectile(GameObject prefab, float speed, float angleOffset)
    {
        if (prefab == null || playerTransform == null)
            return;

        Vector2 baseDirection = ((Vector2)playerTransform.position - GetFirePosition()).normalized;
        Vector2 direction = RotateVector(baseDirection, angleOffset);
        SpawnProjectile(prefab, direction, speed);
    }

    private void SpawnProjectile(GameObject prefab, Vector2 direction, float speed)
    {
        Vector2 spawnPos = GetFirePosition();

        GameObject projectile = Instantiate(prefab, spawnPos, Quaternion.identity);

        BossLaserProjectile bossProjectile = projectile.GetComponent<BossLaserProjectile>();

        if (bossProjectile != null)
        {
            bossProjectile.Initialize(playerTransform, direction, speed);
            return;
        }

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
            projectileRb.linearVelocity = direction * speed;
    }

    private Vector2 GetFirePosition()
    {
        return firePoint != null ? firePoint.position : transform.position;
    }

    private void SwitchToRandomNewEdge()
    {
        CameraEdge newEdge = attachedEdge;

        while (newEdge == attachedEdge)
            newEdge = (CameraEdge)Random.Range(0, 4);

        attachedEdge = newEdge;
        isAttached = false;

        if (attachedEdge == CameraEdge.Top || attachedEdge == CameraEdge.Bottom)
            edgeAxis = Random.Range(-2f, 2f);
        else
            edgeAxis = Random.Range(-1.5f, 1.5f);

        PickNextEdgeSwitchTime();
    }

    private void PickNextEdgeSwitchTime()
    {
        nextEdgeSwitchTime = Time.time + Random.Range(edgeSwitchMinTime, edgeSwitchMaxTime);
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
                }
                else if (edgeAxis < -usableHalfWidth)
                {
                    edgeAxis = -usableHalfWidth;
                    currentStrafeDirection = 1f;
                }
                break;

            case CameraEdge.Left:
            case CameraEdge.Right:
                if (edgeAxis > usableHalfHeight)
                {
                    edgeAxis = usableHalfHeight;
                    currentStrafeDirection = -1f;
                }
                else if (edgeAxis < -usableHalfHeight)
                {
                    edgeAxis = -usableHalfHeight;
                    currentStrafeDirection = 1f;
                }
                break;
        }
    }

    private Vector2 GetAttachedWorldPosition(Vector2 camPos, float usableHalfWidth, float usableHalfHeight)
    {
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

    private void PickNewStrafeDirection()
    {
        currentStrafeDirection = Random.value < 0.5f ? -1f : 1f;
        nextStrafeChangeTime = Time.time + Random.Range(strafeChangeIntervalMin, strafeChangeIntervalMax);
    }

    public void TryDodgeFromBullet(Collider2D other)
    {
        if (isDodging)
            return;

        if (Time.time < nextDodgeTime)
            return;

        if (!other.CompareTag(playerBulletTag))
            return;

        if (Random.value <= dodgeChance)
            StartCoroutine(Dodge());
    }

    private IEnumerator Dodge()
    {
        isDodging = true;
        nextDodgeTime = Time.time + dodgeCooldown;

        Vector2 dodgeDirection;

        if (attachedEdge == CameraEdge.Top || attachedEdge == CameraEdge.Bottom)
            dodgeDirection = Random.value < 0.5f ? Vector2.left : Vector2.right;
        else
            dodgeDirection = Random.value < 0.5f ? Vector2.up : Vector2.down;

        Vector2 start = rb.position;
        Vector2 target = start + dodgeDirection * dodgeDistance;

        float distanceMoved = 0f;

        while (distanceMoved < dodgeDistance)
        {
            Vector2 oldPos = rb.position;
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, dodgeSpeed * Time.fixedDeltaTime));
            distanceMoved += Vector2.Distance(oldPos, rb.position);
            yield return new WaitForFixedUpdate();
        }

        isAttached = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        PickNewStrafeDirection();
        PickNextEdgeSwitchTime();
        isDodging = false;
    }

    private void Face()
    {
        if (!facePlayer || playerTransform == null)
            return;

        Vector2 dir = (Vector2)playerTransform.position - rb.position;

        if (dir.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffset);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDodging)
            return;

        if (Time.time < nextDodgeTime)
            return;

        if (!other.CompareTag(playerBulletTag) &&
            !other.transform.root.CompareTag(playerBulletTag))
            return;

        if (Random.value <= dodgeChance)
            StartCoroutine(Dodge());
    }



    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        ).normalized;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (bodyHitVfx != null)
                bodyHitVfx.SetActive(true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (bodyHitVfx != null && !bodyHitVfx.activeSelf)
                bodyHitVfx.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (bodyHitVfx != null)
                bodyHitVfx.SetActive(false);
        }
    }
}