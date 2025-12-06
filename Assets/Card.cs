using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField, Range(0, 3)] protected int cost;
    public int Cost { get { return cost; } }
    [SerializeField] private Sprite cardImage;
    protected TAG_Outline tagOutline;
    protected TAG_OutlineSelected tagOutlineSelected;

    protected Vector3 originalScale;
    protected Vector3 originalPosition;

    public string cardName;

    [TextArea]
    public string description;

    public bool placed = false;

    protected PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        GetComponentInChildren<CardDescription>().SetCardDescription(cost, cardImage, cardName, description);

        tagOutline = GetComponentInChildren<TAG_Outline>();
        tagOutlineSelected = GetComponentInChildren<TAG_OutlineSelected>();

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    public virtual void EnterHover()
    {
        tagOutline.EnableOutline(true);
        tagOutlineSelected.EnableOutline(false);
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(transform.position + transform.right * -0.25f + transform.up * 0.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public virtual void ExitHover()
    {
        tagOutline.EnableOutline(false);
        tagOutlineSelected.EnableOutline(false);
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(originalPosition, 0.25f).SetEase(Ease.OutBack);
    }

    public virtual void ReadyToUse()
    {
        tagOutlineSelected.EnableOutline(true);
        tagOutline.EnableOutline(false);
    }

    public virtual void UnreadyToUse()
    {
        tagOutlineSelected.EnableOutline(false);
        tagOutline.EnableOutline(true);
    }

    public virtual void UseEffect()
    {
        // Implement card effect here
    }

    public virtual void HideCard(bool hide)
    {
        placed = false;
        transform.DOScale(hide ? Vector3.zero : originalScale, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        {

            placed = true;
        });
    }

    public virtual void DestroyCard()
    {
        placed = false;
        transform.DOMove(originalPosition - Vector3.back * 2f, 0.25f).SetEase(Ease.OutBack);
        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Destroy(gameObject);
            placed = true;
        });
    }

    public virtual void MoveCardToPosition(Vector3 targetPosition, float speed, bool updateOriginalPosition = true)
    {
        placed = false;
        transform.DOMove(targetPosition, speed).SetEase(Ease.OutBack).SetSpeedBased().onComplete = () =>
        {
            placed = true;
            if (updateOriginalPosition)
            {
                originalPosition = transform.localPosition;
                originalScale = transform.localScale;
            }


            ExitHover();
        };

    }
}
