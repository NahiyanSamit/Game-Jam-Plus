using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    private Enemy enemy;
    private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private GameObject[] spikeBalls; // [0] main, [1] secondary
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float attackCooldown = 1.5f;

    private float nextAttackTime;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemy == null) return;

        bool playerInRange = enemy.Distance <= enemy.MaxDistance;

        if (playerInRange && Time.time >= nextAttackTime)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        SpawnProjectiles();
    }

    private void SpawnProjectiles()
    {
        int amount = Random.Range(3, 6);
        float spread = 1.5f;

        for (int i = 0; i < amount; i++)
        {
            // 🔹 First ball → index 0, rest → index 1
            GameObject prefabToSpawn = (i == 0)
                ? spikeBalls[0]
                : spikeBalls[1];

            Vector3 spawnPos =
                transform.position +
                transform.forward * (i + 1f) +
                transform.right * Random.Range(-spread, spread) +
                Vector3.up * 4f;

            GameObject projectile = Instantiate(
                prefabToSpawn,
                spawnPos,
                Quaternion.identity
            );

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(launchPoint.forward * launchForce, ForceMode.Impulse);
            }
        }
    }
}