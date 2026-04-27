using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Spacing")]
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float retreatDistance = 2.5f;
    [SerializeField] private float strafeStrength = 0.6f;
    [SerializeField] private float strafeSwitchInterval = 1.25f;

    [Header("Rotation")]
    [SerializeField] private bool rotateToFacePlayer = true;
    [SerializeField] private float spriteForwardOffset = 0f;

    private Rigidbody2D rb;
    private float strafeTimer;
    private float strafeDirection = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ResolvePlayerReference();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            ResolvePlayerReference();
            ApplyDeceleration();
            return;
        }

        Vector2 enemyPosition = transform.position;
        Vector2 toPlayer = (Vector2)playerTransform.position - enemyPosition;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer < 0.001f)
        {
            ApplyDeceleration();
            return;
        }

        Vector2 directionToPlayer = toPlayer / distanceToPlayer;
        Vector2 radialVelocity = CalculateRadialVelocity(directionToPlayer, distanceToPlayer);
        Vector2 strafeVelocity = CalculateStrafeVelocity(directionToPlayer);

        Vector2 desiredVelocity = Vector2.ClampMagnitude(radialVelocity + strafeVelocity, moveSpeed);
        float accelerationToUse = desiredVelocity.sqrMagnitude < 0.001f ? deceleration : acceleration;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredVelocity, accelerationToUse * Time.fixedDeltaTime);

        if (rotateToFacePlayer)
        {
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + spriteForwardOffset;
            rb.MoveRotation(angle);
        }
    }

    private Vector2 CalculateRadialVelocity(Vector2 directionToPlayer, float distanceToPlayer)
    {
        if (distanceToPlayer > preferredDistance)
        {
            return directionToPlayer * moveSpeed;
        }

        if (distanceToPlayer < retreatDistance)
        {
            return -directionToPlayer * moveSpeed;
        }

        return Vector2.zero;
    }

    private Vector2 CalculateStrafeVelocity(Vector2 directionToPlayer)
    {
        strafeTimer += Time.fixedDeltaTime;
        if (strafeTimer >= strafeSwitchInterval)
        {
            strafeTimer = 0f;
            strafeDirection *= -1f;
        }

        Vector2 tangent = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafeDirection;
        return tangent * (moveSpeed * strafeStrength);
    }

    private void ApplyDeceleration()
    {
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
    }

    private void ResolvePlayerReference()
    {
        if (playerTransform != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject == null)
        {
            playerObject = GameObject.Find("Player");
        }

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
}
