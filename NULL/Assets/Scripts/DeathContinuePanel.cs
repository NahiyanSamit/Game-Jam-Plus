using UnityEngine;

public class DeathContinuePanel : MonoBehaviour
{
    public GameObject panel;

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}