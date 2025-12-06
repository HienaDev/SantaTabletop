using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField, Range(0, 3)] protected int cost;
    protected TAG_Outline tagOutline;

    protected Vector3 originalScale;
    protected Vector3 originalPosition;

    public bool placed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tagOutline = GetComponentInChildren<TAG_Outline>();

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    public virtual void EnterHover()
    {
        tagOutline.EnableOutline(true);
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(transform.position + transform.right * -0.25f,0.2f).SetEase(Ease.OutBack);
    }

    public virtual void ExitHover()
    {
        tagOutline.EnableOutline(false);
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(originalPosition, 0.25f).SetEase(Ease.OutBack);
    }

    public virtual void UseEffect()
    {
        // Implement card effect here
    }


    public virtual void MoveCardToPosition(Vector3 targetPosition, float speed)
    {
        placed = false;
        transform.DOMove(targetPosition, speed).SetEase(Ease.OutBack).SetSpeedBased().onComplete = () =>
        {
            placed = true;
            originalPosition = transform.localPosition;
            originalScale = transform.localScale;
        };
        
    }
}
