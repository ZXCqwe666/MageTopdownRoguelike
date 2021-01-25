using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testProjectile : MonoBehaviour
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
            Debug.Log("Damage done");
            health.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}
