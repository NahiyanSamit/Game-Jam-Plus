using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class IconButton : MonoBehaviour, IPointerClickHandler
{
    public bool isUnlocked = false;   

    private RectTransform rect;
    private Image image;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Gray look when locked
        if (!isUnlocked)
            image.color = new Color(0.6f, 0.6f, 0.6f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            StartCoroutine(ShakeLockedIcon());
            MessageManager.Instance.ShowMessage("Item Not Found", 1.2f);
            return;
        }

        // If unlocked later — you put real actions here
        Debug.Log("Button works now!");
    }

    IEnumerator ShakeLockedIcon()
    {
        Vector3 original = rect.localPosition;

        // Small shake animation
        for (int i = 0; i < 6; i++)
        {
            rect.localPosition = original + new Vector3(Random.Range(-5f, 5f), 0, 0);
            yield return new WaitForSeconds(0.03f);
        }

        rect.localPosition = original;
    }
}

