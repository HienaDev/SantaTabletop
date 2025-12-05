using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FilipeScooter : Bullet
{
    [SerializeField] private bool rightDiagonal;
    public override void MoveBullet()
    {
        // Implement specific movement logic for FilipeScooter here

        Vector2Int leftDiagonalPos = currentPosition + new Vector2Int(-1, -1);
        Vector2Int rightDiagonalPos = currentPosition + new Vector2Int(1, -1);

        bool leftOutside = gridManager.CheckIfPositionOutsideGrid(leftDiagonalPos.x, leftDiagonalPos.y);
        bool rightOutside = gridManager.CheckIfPositionOutsideGrid(rightDiagonalPos.x, rightDiagonalPos.y);


        if(leftOutside && rightOutside)
        {
            BlowBulletUp();
            return;
        }

        if (rightDiagonal)
        {
            if (gridManager.CheckIfPositionOutsideGrid(rightDiagonalPos.x, rightDiagonalPos.y))
            {
                rightDiagonal = false;
            }
            else
            {
                GameObject objAtRightDiagonal = gridManager.GetObjectAtPosition(rightDiagonalPos.x, rightDiagonalPos.y);

                transform.DOMove(gridManager.ConvertPosition(rightDiagonalPos.x, rightDiagonalPos.y), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    currentPosition = rightDiagonalPos;

                    if (objAtRightDiagonal != null)
                    {
                        if (objAtRightDiagonal.TryGetComponent<Present>(out Present present))
                        {
                            int currentPresentHealth = present.DamagePresent();

                            if (currentPresentHealth <= 0)
                            {
                                MoveBullet();
                            }
                            else
                            {
                                BlowBulletUp();
                            }

                            return;
                        }
                    }
                    else
                    {
                        MoveBullet();
                    }
                });
            }
        }
        
        if(!rightDiagonal)
        {
            if (gridManager.CheckIfPositionOutsideGrid(leftDiagonalPos.x, leftDiagonalPos.y))
            {
                rightDiagonal = true;
                MoveBullet();
                return;
            }
            else
            {
                GameObject objAtRightDiagonal = gridManager.GetObjectAtPosition(leftDiagonalPos.x, leftDiagonalPos.y);

                transform.DOMove(gridManager.ConvertPosition(leftDiagonalPos.x, leftDiagonalPos.y), 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    currentPosition = leftDiagonalPos;

                    if (objAtRightDiagonal != null)
                    {
                        if (objAtRightDiagonal.TryGetComponent<Present>(out Present present))
                        {
                            int currentPresentHealth = present.DamagePresent();

                            if (currentPresentHealth <= 0)
                            {
                                MoveBullet();
                            }
                            else
                            {
                                BlowBulletUp();
                            }

                            return;
                        }
                    }
                    else
                    {
                        MoveBullet();
                    }
                });
            }
        }
    }

    public override void OnStopMoveAction()
    {
        Debug.Log("FilipeScooter has stopped moving!");
        // Implement specific logic for when FilipeScooter stops moving here


    }

    public override void DisplayPath()
    {
        Vector2Int currentPos = currentPosition;
        bool currentDirection = rightDiagonal;

        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(currentPos);

        while (true) // Loop runs until a break condition is met
        {
            // 1. Calculate next position
            Vector2Int nextRightPos = currentPos + new Vector2Int(1, -1);
            Vector2Int nextLeftPos = currentPos + new Vector2Int(-1, -1);
            Vector2Int nextPos = currentDirection ? nextRightPos : nextLeftPos;

            // 2. Check if the NEXT position is outside the grid
            bool outsideGrid = gridManager.CheckIfPositionOutsideGrid(nextPos.x, nextPos.y);

            if (outsideGrid)
            {
                // **BOUNCE/END LOGIC**
                Vector2Int oppositePos = currentDirection ? nextLeftPos : nextRightPos;
                bool oppositeOutside = gridManager.CheckIfPositionOutsideGrid(oppositePos.x, oppositePos.y);

                if (oppositeOutside)
                {
                    // Both directions outside, path ends.
                    break;
                }
                else
                {
                    // Bounce: Flip direction and continue prediction from the current cell.
                    currentDirection = !currentDirection;
                    continue;
                }
            }

            // 3. CHECK FOR STOPPING CONDITION (HITTING A PRESENT)
            GameObject objAtNextPos = gridManager.GetObjectAtPosition(nextPos.x, nextPos.y);

            if (objAtNextPos != null && objAtNextPos.TryGetComponent<Present>(out Present present))
            {
                // CRITICAL LOGIC: If the Present has MORE THAN 1 HP, it survives the hit.
                // Since this bullet does only 1 point of damage (implied by your MoveBullet logic),
                // the path prediction must stop if the health is > 1.

                // Assuming Present.Health is the property/field holding the current HP
                if (present.CurrentHealth > 1)
                {
                    // Path stops before damaging the present (since it would survive)
                    // We DO NOT add this cell to the path display, as the bullet would explode.
                    break;
                }
                else
                {
                    // Health is 1 or less: The bullet will destroy the Present and keep moving.
                    // The path continues *through* this cell.
                    path.Add(nextPos);
                }
            }

            // 4. If nothing blocks it, add the position to the path and move on.
            path.Add(nextPos);
            currentPos = nextPos;
        }

        gridManager.TurnOnIndicators(path);
    }


}
