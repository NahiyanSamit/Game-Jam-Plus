using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FunManager : MonoBehaviour
{
    public static FunManager Instance;

    [Header("Fun Settings")]
    public Slider funBar;
    public Image funFill;
    public Outline outlineGlow;
    public float currentFun = 0f;
    public float maxFun = 0f;

    private void Awake()
    {
        Instance = this;
    }

    public void AddFun(float amount)
    {
        currentFun += amount;
        currentFun = Mathf.Clamp(currentFun, 0, maxFun);
        funBar.value = currentFun;

        StartCoroutine(ShakeFunBar());
        StartCoroutine(GlowBar());
        StartCoroutine(PulseGlow());
    }

    IEnumerator ShakeFunBar()
    {
        Vector3 originalScale = funBar.transform.localScale;
        Vector3 big = originalScale * 1.1f;

        funBar.transform.localScale = big;
        yield return new WaitForSeconds(1f);
        funBar.transform.localScale = originalScale;
    }

    IEnumerator GlowBar()
    {
        Color original = funFill.color;
        Color glow = new Color(1f, 1f, 0.5f);

        funFill.color = glow;
        yield return new WaitForSeconds(1f);
        funFill.color = original;
    }

    IEnumerator PulseGlow()
    {
        outlineGlow.enabled = true;
        yield return new WaitForSeconds(0.2f);
        outlineGlow.enabled = false;
    }

    /*SOUND EFFECT
    void PlayFunSound()
    {
        // Only works if you have an AudioManager
        AudioManager.Instance.Play("FunCollect");
    }*/

}
