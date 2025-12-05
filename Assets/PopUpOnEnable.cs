using UnityEngine;
using DG.Tweening; // Don't forget to include the DOTween namespace

public class PopUpOnEnable : MonoBehaviour
{
    [Header("Pop-up Settings")]
    [Tooltip("How long the pop-up animation should take.")]
    [SerializeField] private float duration = 0.2f;

    [Tooltip("The easing type for the animation (e.g., OutBack for a springy effect).")]
    [SerializeField] private Ease easeType = Ease.OutBack;

    private Vector3 originalScale;

    void Awake()
    {
        // Store the object's original scale when the script first loads.
        originalScale = transform.localScale;

        // Ensure the object starts at zero scale, ready for the pop-up.
        transform.localScale = Vector3.zero;
    }

    void OnEnable()
    {
        // Stop any previous tweens on this transform to prevent conflicts.
        transform.DOKill(true);

        // Set the initial scale to zero, just in case.
        transform.localScale = Vector3.zero;

        // Start the tween: scale from zero to the stored original scale.
        transform.DOScale(originalScale, duration)
            .SetEase(easeType);
    }
}