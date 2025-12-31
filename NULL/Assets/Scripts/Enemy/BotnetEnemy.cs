using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Enemy))]
public class BotnetEnemy : MonoBehaviour
{
    private Enemy enemy;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Detection")]
    [SerializeField] private float detectionDistance = 10f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;

    private float nextAttackTime;
    private bool playerDetected;
    private Enemy _enemyScript;
    void Awake()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _enemyScript = GetComponent<Enemy>();
        agent.stoppingDistance = 2f;
        agent.isStopped = true;
    }

    void Update()
    {
        if (enemy == null || enemy.Player == null) return;
        
        if (_enemyScript.DeathTrigger)
        {
            agent.isStopped = true;
            animator.SetTrigger("Death");
        }
        else
        {
            float distance = enemy.Distance;

            // 🔹 Detect player once
            if (!playerDetected && distance <= detectionDistance)
            {
                playerDetected = true;
                agent.isStopped = false;
            }

            // 🔹 Idle
            if (!playerDetected)
            {
                HandleIdle();
                return;
            }

            // 🔹 Chase
            agent.SetDestination(enemy.Player.transform.position);
            animator.SetBool("Run", true);

            // 🔹 Attack only when agent stops
            if (HasReachedTarget())
            {
                HandleAttack();
            }
        }
    }

    // ===== STATES =====

    private void HandleIdle()
    {
        agent.isStopped = true;
        animator.SetBool("Run", false);
    }

    private void HandleAttack()
    {
        agent.isStopped = true;
        animator.SetBool("Run", false);

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger(Random.value > 0.5f ? "Attack01" : "Attack02");
    }

    // ===== HELPERS =====

    private bool HasReachedTarget()
    {
        if (agent.pathPending) return false;
        if (!agent.hasPath) return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }
}
