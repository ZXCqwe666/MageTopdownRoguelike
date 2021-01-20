using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 inputs;
    private readonly float startSpeed = 1f, maxSpeed = 5f, acceleration = 1f, deceleration = 3f;
    private float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = startSpeed;
    }
    void Update()
    {
        inputs.x = Input.GetAxisRaw("Horizontal");
        inputs.y = Input.GetAxisRaw("Vertical");
        inputs.Normalize();

        if (inputs == Vector2.zero && speed > startSpeed)
            speed -= deceleration * Time.deltaTime;
        else if (speed < maxSpeed)
            speed += acceleration * Time.deltaTime;

        rb.velocity = inputs * speed;
    }
}
