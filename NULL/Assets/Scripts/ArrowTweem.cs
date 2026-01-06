using UnityEngine;
using DG.Tweening;

public class ArrowTweem : MonoBehaviour
{
    public float floatHight = 0.4f;
    public float duration = 1.0f;
    void Start()
    {
        transform.DOLocalMoveY(transform.localPosition.y + floatHight, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
        
    }

    
}
