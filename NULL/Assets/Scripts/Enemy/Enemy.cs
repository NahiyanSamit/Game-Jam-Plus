using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private GameObject player;
    private Transform enemyPosition;
    [SerializeField]private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int health;

    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float distance;

    public float Distance
    {
        get => distance;
        set => distance = value;
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
        }

    }

    private void Death()
    {
        
        if (health <= 0)
        {
            Debug.Log("Enemy will die");
        }
    }
}