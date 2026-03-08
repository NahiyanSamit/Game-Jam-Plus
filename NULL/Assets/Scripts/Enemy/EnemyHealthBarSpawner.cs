using UnityEngine;

public class EnemyHealthBarSpawner : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform healthBarAnchor;
    public Transform canvasParent;

    void Start()
    {
        GameObject bar = Instantiate(healthBarPrefab, canvasParent);

        bar.GetComponent<HealthBarFollow>().target = healthBarAnchor;

        bar.GetComponent<EnemyHealthBar>().SetHealth(GetComponent<Health>());
    }
}