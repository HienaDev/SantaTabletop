using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EndTurnBell : MonoBehaviour
{

    private Vector3 originalScale;
    private Vector3 originalRotation;
    private Vector3 originalPosition;

    private bool hovered = false;

    private PlayerController playerController;

    private Outline outline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        outline = GetComponent<Outline>();

        originalPosition
            = transform.position;
        originalRotation
            = transform.eulerAngles;
        originalScale
            = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered = true && !playerController.turnHappening)
        {
            ExitHover(true);
        }

        // While hovered slightly rotate and bob the bell
        if (hovered)
        {
            float rotationAmount = Mathf.Sin(Time.time * 5f) * 5f;
            float bobAmount = Mathf.Sin(Time.time * 5f) * 0.1f;
            transform.eulerAngles = originalRotation + new Vector3(0, 0, rotationAmount);
            transform.position = originalPosition + new Vector3(0, bobAmount + 1.5f, 0);
        }
    }

    public void EnterHover()
    {
        if (!playerController.turnHappening)
            return;

        outline.enabled = true;

        hovered = false;
        transform.DOKill();
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(originalPosition + new Vector3(0, 1.5f, 0), 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            hovered = true;
        });
    }

    public void ExitHover(bool force = false)
    {
        if (!playerController.turnHappening && !force)
            return;

        outline.enabled = false;

        hovered = false;
        transform.DOKill();
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        transform.DOMove(originalPosition, 0.2f).SetEase(Ease.OutBack);
        transform.DORotate(originalRotation, 0.2f).SetEase(Ease.OutBack);
    }
}
