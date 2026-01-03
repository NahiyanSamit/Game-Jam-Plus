using SmallHedge.SoundManager;
using UnityEngine;

public class AtomBallPower : SpikeBallPower
{
    [Header("Homing Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 3f;
    [SerializeField] private int damage = 4;
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SmallHedge.SoundManager.SoundManager.PlaySound(SoundType.PLAYERHIT);
            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player hit by AtomBall");
            }

            Destroy(gameObject); // destroy projectile
        }
    }
    
    
}