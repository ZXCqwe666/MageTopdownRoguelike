using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public int speed = 10;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Health health))
        {
            health.TakeDamage(5);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 8)
            Destroy(gameObject);
    }
}
