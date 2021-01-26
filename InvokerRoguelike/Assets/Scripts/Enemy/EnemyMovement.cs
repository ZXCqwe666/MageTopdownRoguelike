using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    public LayerMask obstacle, enemyObstacle;

    public float speed = 5f;

    private void Start()
    {   
        player = FindObjectOfType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.velocity = GetFinalMovementIntention() * speed; 
    }
    private Vector2 GetFinalMovementIntention()
    {
        Vector3 intention = GetMovementIntention() + AvoidObstacle();
        if (intention.magnitude < 0.5f)
            return Vector3.zero;
        return intention.normalized;
    }
    private Vector3 GetMovementIntention()
    {
        Vector3 intention = Vector3.zero;
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        float targetDistance = 1f; //(minimal distance to object) PROBABLY CHANGE
        float springStrength = distance - targetDistance; 
        intention += direction * springStrength;
        return intention;
    }
    private Vector3 AvoidObstacle()
    {
        Vector3 intention = Vector3.zero;

        Collider2D[] hitCollidersEnemy = Physics2D.OverlapCircleAll(transform.position, 5f, enemyObstacle); // change 5f 
        float enemiesArround = hitCollidersEnemy.Length;
        foreach (Collider2D collider in hitCollidersEnemy)
        {
            if (collider.transform.position == transform.position) continue;

            Vector3 colliderClosestPoint = collider.ClosestPoint(transform.position);
            Vector3 direction = (colliderClosestPoint - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, colliderClosestPoint);

            float springStreangth = 1f / (1f + distance * distance * distance);
            intention -= direction * springStreangth;
        }

        Collider2D [] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5f, obstacle); // change 5f 
        float obstaclesArround = hitColliders.Length;
        float strenghtAmplify = 1f;
        if (obstaclesArround != 0)
            strenghtAmplify = enemiesArround / obstaclesArround;

        foreach (Collider2D collider in hitColliders)
        {
            Vector3 colliderClosestPoint = collider.ClosestPoint(transform.position);
            Vector3 direction = (colliderClosestPoint - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, colliderClosestPoint);

            float springStreangth = 1f / (1f + distance * distance * distance);
            intention -= direction * springStreangth * strenghtAmplify;
        }
        return intention;
    }
}
