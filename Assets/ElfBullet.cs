using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ElfBullet : Bullet
{


    public override void MoveBullet()
    {
        moving = true;
        Vector2Int targetPos = currentPosition + new Vector2Int(0, -1);

        if (gridManager.CheckIfPositionOutsideGrid(targetPos.x, targetPos.y))
        {
            BlowBulletUp();
            return;
        }

        GameObject objAtTarget = gridManager.GetObjectAtPosition(targetPos.x, targetPos.y);

        transform.DOMove(gridManager.ConvertPosition(targetPos.x, targetPos.y), 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            currentPosition = targetPos;

            if (objAtTarget != null)
            {
                if (objAtTarget.TryGetComponent<Present>(out Present present))
                {
                    int currentPresentHealth = present.DamagePresent();

                    if (FindAnyObjectByType<PlayerController>().KingOfElvesBuff && currentPresentHealth > 0)
                    {
                        present.DamagePresent();
                    }

                    BlowBulletUp();

                    return;
                }
            }
            else
            {
                MoveBullet();
            }
        });


    }

    public override void DisplayPath()
    {
        if (moving)
            return;

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
            

            GameObject objAtTarget = gridManager.GetObjectAtPosition(targetPos.x, targetPos.y);

            if (objAtTarget == null)
            {
                path.Add(targetPos);
                continue;
            }

            if (objAtTarget.GetComponent<Present>() != null)
            {
                path.Add(targetPos);
                break;
            }

            
        }

        gridManager.TurnOnPathIndicators(path);
    }
}
