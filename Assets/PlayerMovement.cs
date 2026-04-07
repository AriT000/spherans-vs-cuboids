using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;                  // max speed player can reach
    public float acceleration = 18f;              // how quickly player speeds up
    public float deceleration = 22f;              // how quickly player slows down
    public float reverseAcceleration = 10f;       // slower acceleration when completely reversing direction

    private Rigidbody2D rb;
    private Vector2 movementInput;                     // raw player input
    private PlayerInputActions inputActions;

    void Awake()
    {
        // initialize input system
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Read movement input
        // Normalized so diagonal movement isn't faster than straight movement
        movementInput = inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = movementInput * moveSpeed;  // target is how fast the player wants to go
        Vector2 currentVelocity = rb.linearVelocity;         // current is how fast the player is going
        float accelToUse = acceleration;                     // default acceleration

        // if player goes turns opposite direction, lower acceleration so movement feels heavier
        if (movementInput != Vector2.zero && currentVelocity != Vector2.zero)
        {
            float dot = Vector2.Dot(currentVelocity.normalized, movementInput);
            if (dot < -0.25f)
            {
                accelToUse = reverseAcceleration;
            }
        }

        // if no input, decelerate toward zero
        if (movementInput == Vector2.zero)
        {
            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, accelToUse * Time.fixedDeltaTime);
        }
    }
}
