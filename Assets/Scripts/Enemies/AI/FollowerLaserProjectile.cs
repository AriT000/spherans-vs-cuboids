using UnityEngine;
using Assets.Scripts.Entities;

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
    [SerializeField] private float turnSpeed = 4f;
    [SerializeField] private float lockRadius = 1.5f;
    [SerializeField] private float lifetime = 5f;

    [Header("Damage")]
    [SerializeField] private int fallbackDamage = 1;
    [SerializeField] private bool destroyOnHit = true;

    [Header("Facing")]
    [SerializeField] private float spriteForwardOffset = 0f;

    private Transform player;
    private Vector2 moveDirection;
    private ProjectileState state = ProjectileState.Homing;
    private float lifeTimer;
    private bool hasHitPlayer;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        FindPlayerIfMissing();

        if (moveDirection.sqrMagnitude < 0.001f)
        {
            if (player != null)
                moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
            else
                moveDirection = transform.right;
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (state == ProjectileState.Homing)
            HomingMove();

        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        FaceMoveDirection();
    }

    private void HomingMove()
    {
        FindPlayerIfMissing();

        if (player == null)
            return;

        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;

        if (toPlayer.magnitude <= lockRadius)
        {
            state = ProjectileState.Locked;
            return;
        }

        Vector2 desiredDirection = toPlayer.normalized;

        moveDirection = Vector2.Lerp(
            moveDirection,
            desiredDirection,
            turnSpeed * Time.deltaTime
        ).normalized;
    }

    public void Initialize(Transform target)
    {
        player = target;

        if (player != null)
            moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        else
            moveDirection = transform.right;
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