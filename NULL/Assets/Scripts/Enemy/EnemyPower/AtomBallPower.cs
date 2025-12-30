using UnityEngine;

public class AtomBallPower : SpikeBallPower
{
    [Header("Homing Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 3f;

    protected override void Start()
    {
        base.Start();

        // Initial velocity
        rb.linearVelocity = transform.forward * moveSpeed;
    }

    void FixedUpdate()
    {
        if (rb == null || player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;

        Vector3 newVelocity = Vector3.RotateTowards(
            rb.linearVelocity.normalized,
            direction,
            turnSpeed * Time.fixedDeltaTime,
            0f
        ) * moveSpeed;

        rb.linearVelocity = newVelocity;

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }
}