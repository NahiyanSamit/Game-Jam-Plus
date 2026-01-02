using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private GameObject player;
    private Transform enemyPosition;
    [SerializeField]private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int health;
    private Health healthScript;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float distance;
    private bool deathTrigger = false;
    private bool detected = false;

    public bool Detected
    {
        get => detected;
        set => detected = value;
    }
    public float Distance
    {
        get => distance;
        set => distance = value;
    }

    public bool DeathTrigger
    {
        get => deathTrigger;
        set => deathTrigger = value;
    }
    public GameObject Player
    {
        get => player;
        set => player = value;
    }
    public float MaxDistance
    {
        get => maxDistance;
        set => maxDistance = value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player");
        healthScript= GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        LookingAtPlayer();
        Death();
    }


    private void LookingAtPlayer()
    {
        if (player != null && distance <= maxDistance)
        {
            transform.LookAt(player.transform);
            detected = true;
        }

    }

    private void Death()
    {
        
        if (healthScript.CurrentHealth <= 0)
        {
            deathTrigger = true;
            Debug.Log("Enemy will die");
            OnDeath();
        }
        
    }
    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} died");

        // OPTIONAL: disable collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // OPTIONAL: stop NavMesh
        if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
            agent.isStopped = true;

        // OPTIONAL: play animation
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        Destroy(gameObject, 5f);
    }
}