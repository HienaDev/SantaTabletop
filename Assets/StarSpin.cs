using UnityEngine;
using DG.Tweening;
using System.Net.NetworkInformation;

public class StarSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    public float initialSpeed = 1080f;    // degrees per second at start
    public float finalSpeed = 180f;       // ending speed after slowing
    public float slowDownDuration = 1.5f; // how long it takes to slow down

    [Header("Lifetime Settings")]
    public float totalDegreesBeforeDestroy = 720f; // e.g. two full spins
    private float accumulatedDegrees = 0f;

    private float currentSpeed;
    private Tween speedTween;

    private bool dying = false;

    void OnEnable()
    {
        accumulatedDegrees = 0f;
        currentSpeed = initialSpeed;

        // Tween that slows down speed over time
        speedTween = DOTween.To(
            () => currentSpeed,
            v => currentSpeed = v,
            finalSpeed,
            slowDownDuration
        ).SetEase(Ease.OutQuad);
    }

    void Update()
    {
        // Rotate manually based on currentSpeed
        float deltaRotation = currentSpeed * Time.deltaTime;
        transform.Rotate(0f, deltaRotation, 0f, Space.Self);

        // Track accumulated rotation
        accumulatedDegrees += Mathf.Abs(deltaRotation);

        // Check if we reached the limit
        if (accumulatedDegrees >= totalDegreesBeforeDestroy && !dying)
        {
            dying = true;
            speedTween?.Kill();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    void OnDisable()
    {
        speedTween?.Kill();
    }
}
