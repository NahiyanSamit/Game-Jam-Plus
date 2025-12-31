using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health health;
    //private Camera cam;

    void Start()
    {
        //cam = Camera.main;
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
    }

    void Update()
    {
        slider.value = health.CurrentHealth;
       // transform.forward = cam.transform.forward;
    }
}