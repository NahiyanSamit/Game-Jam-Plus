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
            return;
        }

        if (!enemy.Detected)
        {
            agent.isStopped = true;
            animator.SetBool("Run", false);
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(enemy.Player.transform.position);
        animator.SetBool("Run", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        DamageDealer dealer = GetComponent<DamageDealer>();
        Health playerHealth = collision.gameObject.GetComponent<Health>();

        if (dealer != null && playerHealth != null)
            playerHealth.TakeDamage(dealer.Damage);
    }
}