using UnityEngine;

public class PlayerDamageDebug : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("PLAYER trigger entered by: " + other.name);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("PLAYER trigger staying with: " + other.name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("PLAYER collision entered by: " + collision.gameObject.name);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("PLAYER collision staying with: " + collision.gameObject.name);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("PLAYER particle hit by: " + other.name);
    }
}