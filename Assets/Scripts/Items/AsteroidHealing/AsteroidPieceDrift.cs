using UnityEngine;

public class AsteroidPieceDrift : MonoBehaviour
{
    [SerializeField] private float driftSpeed = 2f;
    [SerializeField] private float rotateSpeed = 90f;

    private Vector2 driftDirection;

    private void Awake()
    {
        driftDirection = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        transform.position += (Vector3)(driftDirection * driftSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}