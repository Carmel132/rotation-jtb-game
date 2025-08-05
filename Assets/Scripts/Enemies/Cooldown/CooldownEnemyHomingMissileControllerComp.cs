using System.Collections.Generic;
using UnityEngine;

public class CooldownEnemyHomingMissileControllerComp : MonoBehaviour
{
    public Vector3 target;
    public Quaternion rotation;
    [SerializeField]
    float searchRadius = 2f;
    [SerializeField]
    float moveSpeed = 4f;
    [SerializeField]
    float angularVelocity = 4f;
    ContactFilter2D filter;
    [SerializeField]
    float lifetime = 60f;

    void Start()
    {
        filter = new();
        filter.NoFilter();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        detectPlayer();
        moveToTarget();
    }

    void detectPlayer()
    {
        List<Collider2D> collisions = new();
        Physics2D.OverlapCircle(transform.position, searchRadius, filter, collisions);
        foreach (Collider2D collision in  collisions)
        {
            if (collision.CompareTag("Player")) {
                target = collision.transform.position;
            }
        }
    }

    void moveToTarget()
    {
        //Debug.DrawLine(transform.position, target, Color.red);
        Vector2 toTarget = (target - transform.position).normalized;
        Vector2 forward = transform.right;

        float angleToTarget = Mathf.Atan2(toTarget.y, toTarget.x);
        float angleCurrent = Mathf.Atan2(forward.y, forward.x);
        float angleDelta = Mathf.DeltaAngle(Mathf.Rad2Deg * angleCurrent, Mathf.Rad2Deg * angleToTarget);

        float maxStep = angularVelocity * Time.deltaTime;
        float step = Mathf.Clamp(angleDelta, -maxStep, maxStep);

        transform.Rotate(0, 0, step);

        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }
}
