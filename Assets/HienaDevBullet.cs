using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HienaDevBullet : Bullet
{

    [SerializeField] private Vector3 customInitialRotation = new Vector3(0, 0, -60);

    public override void MoveBullet()
    {
        // Spyglass Falling Animation

        transform.eulerAngles = customInitialRotation - Vector3.forward * 10f;
        transform.localScale = Vector3.zero;

        Vector3 rotationToAdd = customInitialRotation * -1f;

        transform.DORotate(customInitialRotation, 1.5f).SetEase(Ease.OutCirc);
        transform.DOScale(originalScale, 1.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            transform.DORotate(rotationToAdd, 1f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce, overshoot: 0.1f).OnComplete(() =>
            {
                gridManager.DamageAllPresentsInCollumn(currentPosition.x);
                BlowBulletUp();
            });
        });


    }

    public override void DisplayPath()
    {
        if (moving)
            return;

        transform.eulerAngles = customInitialRotation;
        transform.localScale = new Vector3(0.1f, 1f, 1f);

        Vector2Int targetPos = currentPosition;

        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(currentPosition);

        while (true)
        {

            targetPos += new Vector2Int(0, -1);

            Debug.Log("Checking position: " + targetPos);
            if (gridManager.CheckIfPositionOutsideGrid(targetPos.x, targetPos.y))
            {
                break;
            }

            // Here you can add code to visually display the path if needed

            path.Add(targetPos);
        }

        gridManager.TurnOnPathIndicators(path);
    }
}
