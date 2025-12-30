using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpikeBallPower : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] protected float throwForce = 20f;
    [SerializeField] protected float upwardForce = 2f;
    [SerializeField] protected float lifeTime = 5f;

    protected Rigidbody rb;
    protected Transform player;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Start()
    {
        ThrowTowardPlayer();
        Destroy(gameObject, lifeTime);
    }

    protected void ThrowTowardPlayer()
    {
        if (rb == null || player == null) return;

        // Direction toward player (horizontal aim)
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        direction.Normalize();

        // Final throw vector (arc-like throw)
        Vector3 force =
            direction * throwForce +
            Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.Impulse);
    }
}