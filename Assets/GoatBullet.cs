using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GoatBullet : Bullet
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

        transform.DOMove(gridManager.ConvertPosition(targetPos.x, targetPos.y), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            currentPosition = targetPos;

            if (objAtTarget != null)
            {
                if (objAtTarget.TryGetComponent<Present>(out Present present))
                {
                    //int currentPresentHealth = present.DamagePresent();

                    // Target all presents in a 3x3 grid
                    
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Vector2Int adjacentPos = new Vector2Int(targetPos.x - 1 + i, targetPos.y - 1 + j);
                            if (gridManager.CheckIfPositionOutsideGrid(adjacentPos.x, adjacentPos.y))
                                continue;
                            GameObject adjacentObj = gridManager.GetObjectAtPosition(adjacentPos.x, adjacentPos.y);
                            if (adjacentObj != null && adjacentObj.TryGetComponent<Present>(out Present adjacentPresent))
                            {
                                adjacentPresent.DamagePresent();
                            }
                        }
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

                // Add all adjacent positions to the path in a 3x3 grid
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2Int adjacentPos = new Vector2Int(targetPos.x - 1 + i, targetPos.y - 1 + j);
                        if (gridManager.CheckIfPositionOutsideGrid(adjacentPos.x, adjacentPos.y))
                            continue;
                        if (!path.Contains(adjacentPos))
                        {
                            path.Add(adjacentPos);
                        }
                    }
                }

                break;
            }


        }

        gridManager.TurnOnPathIndicators(path);
    }
}
