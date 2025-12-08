using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FilipeScooter : Bullet
{
    [SerializeField] private bool rightDiagonal;

    // ----------------- MAIN BULLET MOVEMENT -----------------
    public override void MoveBullet()
    {
        moving = true;

        Vector2Int leftDiagonalPos = currentPosition + new Vector2Int(-1, -1);
        Vector2Int rightDiagonalPos = currentPosition + new Vector2Int(1, -1);

        bool leftOutside = gridManager.CheckIfPositionOutsideGrid(leftDiagonalPos.x, leftDiagonalPos.y);
        bool rightOutside = gridManager.CheckIfPositionOutsideGrid(rightDiagonalPos.x, rightDiagonalPos.y);

        // ----------------- END OF GRID CHECK -----------------
        if (leftOutside && rightOutside)
        {
            BlowBulletUp(); // destroy or stop
            return;
        }

        Vector2Int nextPos;

        // ----------------- DETERMINE NEXT TILE AND BOUNCE -----------------
        if (rightDiagonal)
        {
            nextPos = rightDiagonalPos;
            if (rightOutside)
            {
                // Bounce: flip direction
                rightDiagonal = false;
                nextPos = leftDiagonalPos;

                // Accumulate rotation: right → left
                transform.Rotate(0f, -90f, 0f);
            }
        }
        else
        {
            nextPos = leftDiagonalPos;
            if (leftOutside)
            {
                // Bounce: flip direction
                rightDiagonal = true;
                nextPos = rightDiagonalPos;

                // Accumulate rotation: left → right
                transform.Rotate(0f, 90f, 0f);
            }
        }

        // ----------------- MOVE -----------------
        GameObject objAtTarget = gridManager.GetObjectAtPosition(nextPos.x, nextPos.y);
        Vector3 targetWorld = gridManager.ConvertPosition(nextPos.x, nextPos.y);

        transform.DOMove(targetWorld, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            currentPosition = nextPos;

            if (objAtTarget != null && objAtTarget.TryGetComponent<Present>(out Present present))
            {
                int currentPresentHealth = present.DamagePresent();
                if (currentPresentHealth <= 0)
                    MoveBullet(); // continue moving
                else
                    BlowBulletUp();

                return;
            }

            // Continue moving in the current direction
            MoveBullet();
        });

        transform.DOLookAt(targetWorld, 0.3f);
    }

    public override void OnStopMoveAction()
    {
        Debug.Log("FilipeScooter has stopped moving!");
    }

    public override void DisplayPath()
    {
        if (moving)
            return;

        

        Vector2Int currentPos = currentPosition;
        bool currentDirection = rightDiagonal;

        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(currentPos);

        while (true)
        {
            Vector2Int nextRightPos = currentPos + new Vector2Int(1, -1);
            Vector2Int nextLeftPos = currentPos + new Vector2Int(-1, -1);
            Vector2Int nextPos = currentDirection ? nextRightPos : nextLeftPos;

            bool outsideGrid = gridManager.CheckIfPositionOutsideGrid(nextPos.x, nextPos.y);

            if (outsideGrid)
            {
                Vector2Int oppositePos = currentDirection ? nextLeftPos : nextRightPos;
                bool oppositeOutside = gridManager.CheckIfPositionOutsideGrid(oppositePos.x, oppositePos.y);

                if (oppositeOutside)
                    break;

                currentDirection = !currentDirection; // bounce
                continue;
            }

            GameObject objAtNextPos = gridManager.GetObjectAtPosition(nextPos.x, nextPos.y);

            if (objAtNextPos != null && objAtNextPos.TryGetComponent<Present>(out Present present))
            {
                path.Add(nextPos);
                if (present.CurrentHealth > 1)
                    break;
            }

            path.Add(nextPos);
            currentPos = nextPos;
        }



        transform.LookAt(gridManager.ConvertPosition(path[1].x, path[1].y));

        gridManager.TurnOnPathIndicators(path);
    }
}
