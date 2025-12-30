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
    private bool isDead;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;
        if (enemy == null || enemy.Player == null) return;

        float distance = enemy.Distance;

        // Detect player
        if (!playerDetected && distance <= detectionDistance)
        {
            playerDetected = true;
            agent.isStopped = false; // IMPORTANT
        }

        // Idle until detected
        if (!playerDetected)
        {
            HandleIdle();
            return;
        }

        // Move first
        HandleRun();

        // Attack only AFTER reaching target
        if (HasReachedTarget())
        {
            HandleAttack();
        }
    }

    // ================= STATES =================

    void HandleIdle()
    {
        agent.isStopped = true;
        animator.SetBool("Run", false);
    }

    void HandleRun()
    {
         animator.SetBool("Run", true);
        agent.speed = 4.5f;
        agent.isStopped = false;
        agent.SetDestination(enemy.Player.transform.position);

       
    }

    void HandleAttack()
    {
        agent.isStopped = true;
        animator.SetBool("Run", false);

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger(Random.value > 0.5f ? "Attack01" : "Attack02");
    }

    // ================= HELPERS =================

    bool HasReachedTarget()
    {
        if (!agent.hasPath) return false;
        if (agent.pathPending) return false;

        return agent.remainingDistance <= agent.stoppingDistance;
    }
}
