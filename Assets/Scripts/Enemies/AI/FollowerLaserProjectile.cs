using UnityEngine;
using Assets.Scripts.Entities;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FollowerLaserProjectile : MonoBehaviour
{
    private enum ProjectileState
    {
        Homing,
        Locked
    }

    [Header("Movement")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float lockRadius = 1.5f;
    [SerializeField] private float lockedLifetime = 2f;

    [Header("Damage")]
    [SerializeField] private int fallbackDamage = 1;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Facing")]
    [SerializeField] private float spriteForwardOffset = 0f;

    private Rigidbody2D rb;
    private Transform player;
    private ProjectileState state = ProjectileState.Homing;

    private Vector2 moveDirection;
    private float lockedTimer;
    private bool hasHitPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        rb.gravityScale = 0f;
    }

    private void Start()
    {
        if (player == null)
            FindPlayerIfMissing();

        if (player != null)
            moveDirection = ((Vector2)player.position - rb.position).normalized;
        else
            moveDirection = transform.right;
    }

    private void FixedUpdate()
    {
        if (state == ProjectileState.Homing)
            HomingMove();
        else
            LockedMove();

        rb.linearVelocity = moveDirection * speed;
        FaceMoveDirection();
    }

    private void HomingMove()
    {
        FindPlayerIfMissing();

        if (player == null)
            return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= lockRadius)
        {
            state = ProjectileState.Locked;
            lockedTimer = 0f;
            return;
        }

        Vector2 desiredDirection = toPlayer.normalized;

        moveDirection = Vector2.Lerp(
            moveDirection,
            desiredDirection,
            turnSpeed * Time.fixedDeltaTime
        ).normalized;
    }

    private void LockedMove()
    {
        lockedTimer += Time.fixedDeltaTime;

        if (lockedTimer >= lockedLifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer)
            return;

        if (!other.CompareTag("Player"))
            return;

        AttributesManager playerAttributes = other.GetComponent<AttributesManager>();

        if (playerAttributes == null)
            playerAttributes = other.GetComponentInParent<AttributesManager>();

        if (playerAttributes == null)
            return;

        hasHitPlayer = true;

        CallPrivateTakeDamage(playerAttributes, fallbackDamage);

        if (destroyOnHit)
            Destroy(gameObject);
    }

    private void FaceMoveDirection()
    {
        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffset);
    }

    private void FindPlayerIfMissing()
    {
        if (player != null)
            return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    public void Initialize(Transform target)
    {
        player = target;

        if (player != null)
            moveDirection = ((Vector2)player.position - rb.position).normalized;
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
}