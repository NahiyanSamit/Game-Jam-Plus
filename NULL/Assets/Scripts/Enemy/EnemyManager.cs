using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent;
    [SerializeField] private float chaseRange = 20f,nearbyDistance = 5f;
    [SerializeField] private int HeartLevel=5;
    private float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent= GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
         distance = Vector3.Distance(transform.position, target.position);
         ChasePlayer();
         AttackPlayer();
         //Death();
    }

    private void SimpleMovement()
    {
        agent.ResetPath();
        
    }

    private void ChasePlayer()
    {
        
        if (distance <= chaseRange)
        {
            transform.LookAt(target);
            agent.SetDestination(target.position);
            
        }
        else
        {
            SimpleMovement();
        }
    }

    private void Death()
    {
        //Death of the enemy
        if(HeartLevel<=0)
            Destroy(gameObject);
            Debug.Log("Death");
    }

    private void AttackPlayer()
    {
        agent.transform.LookAt(target);
        if (distance <= nearbyDistance)
        {
            agent.acceleration = 2f;
        }
       
    }

    private void HealthDamaege()
    {
        
    }
    
}
