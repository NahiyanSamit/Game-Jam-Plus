using UnityEngine;
using System.Collections;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    [Header("UI References")]
    public GameObject messagePanel;
    public TMP_Text messageText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        messagePanel.SetActive(true);
        messageText.text = msg;
        StopAllCoroutines();
        StartCoroutine(HideAfter(duration));
    }

    private IEnumerator HideAfter(float time)
    {
        yield return new WaitForSeconds(time);
        messagePanel.SetActive(false);
    }
}

